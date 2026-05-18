#!/usr/bin/env python3
"""Fix missing skipEndpointValidation parameter forwarding in C# files."""

import re
from pathlib import Path

def fix_file(filepath):
    """Fix a single file."""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    original_content = content
    lines = content.split('\n')
    result = []
    i = 0
    fixes = 0

    while i < len(lines):
        line = lines[i]
        result.append(line)

        # Check if this line contains skipEndpointValidation parameter
        if 'bool skipEndpointValidation = false,' in line:
            # Look ahead for return statement
            j = i + 1
            while j < len(lines) and j < i + 20:
                if 'return' in lines[j] and 'Assert' in lines[j]:
                    # Found return, check if we need to add skipEndpointValidation

                    # Scan forward from return to find if skipEndpointValidation is already there
                    k = j
                    has_skip = False
                    found_caller_file = False
                    insert_pos = -1

                    while k < len(lines) and k < j + 30:
                        if 'skipEndpointValidation: skipEndpointValidation' in lines[k]:
                            has_skip = True
                            break
                        if 'callerFilePath:' in lines[k]:
                            found_caller_file = True
                            insert_pos = k
                            break
                        if lines[k].strip().endswith(');'):
                            break
                        k += 1

                    # If we don't have skipEndpointValidation but found callerFilePath, insert it
                    if not has_skip and found_caller_file and insert_pos > 0:
                        # Continue adding lines until we reach the insert position
                        while i < insert_pos - 1:
                            i += 1
                            result.append(lines[i])

                        # Add the next line (should be just before callerFilePath)
                        i += 1
                        result.append(lines[i])

                        # Now add skipEndpointValidation before callerFilePath
                        indent = len(lines[insert_pos]) - len(lines[insert_pos].lstrip())
                        skip_line = ' ' * indent + 'skipEndpointValidation: skipEndpointValidation,'
                        result.append(skip_line)
                        fixes += 1

                    break
                j += 1

        i += 1

    if fixes > 0:
        new_content = '\n'.join(result)
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f"✓ {filepath.name}: {fixes} fixes")
        return fixes
    else:
        return 0

def main():
    base_dir = Path('src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions')
    files_to_fix = [
        'Client.Assert.Delete.AsError.cs',
        'Client.Assert.Delete.cs',
        'Client.Assert.Get.AsError.cs',
        'Client.Assert.Get.cs',
        'Client.Assert.HttpCall.cs',
        'Client.Assert.Patch.NoContent.cs',
        'Client.Assert.Post.NoContent.cs',
        'Client.Assert.Put.AsError.cs',
        'Client.Assert.Put.NoContent.cs',
    ]

    total_fixes = 0
    for filename in files_to_fix:
        filepath = base_dir / filename
        if filepath.exists():
            fixes = fix_file(filepath)
            total_fixes += fixes
        else:
            print(f"✗ {filename}: not found")

    print(f"\nTotal fixes: {total_fixes}")

if __name__ == '__main__':
    main()
