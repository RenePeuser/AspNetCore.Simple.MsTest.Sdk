"""Line-ending- and BOM-preserving exact-string editor."""
import io, sys

def read(path):
    raw = io.open(path, 'rb').read()
    bom = raw.startswith(b'\xef\xbb\xbf')
    text = raw.decode('utf-8-sig')
    crlf = '\r\n' in text
    return text.replace('\r\n', '\n'), bom, crlf

def write(path, text, bom, crlf):
    out = text.replace('\n', '\r\n') if crlf else text
    data = out.encode('utf-8')
    if bom:
        data = b'\xef\xbb\xbf' + data
    io.open(path, 'wb').write(data)

def sub(path, old, new, count=1, required=True):
    text, bom, crlf = read(path)
    n = text.count(old)
    if n != count:
        if required:
            raise SystemExit('MISMATCH in %s: expected %d occurrence(s), found %d for:\n%s' % (path, count, n, old[:200]))
        return False
    write(path, text.replace(old, new), bom, crlf)
    print('  patched %s' % path)
    return True
