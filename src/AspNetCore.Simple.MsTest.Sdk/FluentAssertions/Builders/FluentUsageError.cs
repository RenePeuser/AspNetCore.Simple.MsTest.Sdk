using System.Globalization;
using System.Reflection;
using System.Text;
using System;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Builds the message for a fluent chain that is used wrongly in a way no type-state can prevent, because
    /// it depends on a runtime value (a parameter or header name given twice). Same layout as the sdk's
    /// assertion output: what happened, why it is a problem, how to fix it, and where in the test.
    /// </summary>
    internal static class FluentUsageError
    {
        private const string Separator = "──────────────────────────────────────────────────────────────";

        private const string Banner = "══════════════════════════════════════════════════════════════";

        internal static ArgumentException DuplicateParameter(string entryPoint,
                                                             string placeholder,
                                                             object? firstValue,
                                                             object? secondValue,
                                                             Assembly callingAssembly,
                                                             string callerFilePath,
                                                             string callerMemberName,
                                                             int callerLineNumber)
        {
            var nakedName = placeholder.Trim('$');

            return Build(title: "PLACEHOLDER PARAMETER SET TWICE",
                         what: [$"{"Parameter",-10} : {placeholder}", $"{"First",-10} : {Format(firstValue)}", $"{"Second",-10} : {Format(secondValue)}"],
                         why: ["A placeholder is replaced by exactly ONE value. The engine takes the first one and the",
                               "second is silently dropped - the request would not send what the test says it sends."],
                         fix: ["Set every parameter once per chain. To send another value, branch a stored stage:"],
                         fixCode: ["var request = " + entryPoint + ";",
                                   "",
                                   "await request.WithParameter(\"" + nakedName + "\", " + Format(firstValue) + ")…ExecuteAsync();",
                                   "await request.WithParameter(\"" + nakedName + "\", " + Format(secondValue) + ")…ExecuteAsync();"],
                         callingAssembly,
                         callerFilePath,
                         callerMemberName,
                         callerLineNumber);
        }

        internal static ArgumentException DuplicateHeader(string header,
                                                          string firstValue,
                                                          string secondValue,
                                                          Assembly callingAssembly,
                                                          string callerFilePath,
                                                          string callerMemberName,
                                                          int callerLineNumber)
        {
            return Build(title: "REQUEST HEADER SET TWICE",
                         what: [$"{"Header",-10} : {header}", $"{"First",-10} : {Format(firstValue)}", $"{"Second",-10} : {Format(secondValue)}"],
                         why: ["Header names are case-insensitive and a header is set once per chain. Keeping only one",
                               "of the two values would silently send something else than the test reads."],
                         fix: ["Set the header once. For several values send them as one comma-separated value,",
                               "for another value branch a stored stage:"],
                         fixCode: [".WithHeader(\"" + header + "\", \"" + firstValue + ", " + secondValue + "\")"],
                         callingAssembly,
                         callerFilePath,
                         callerMemberName,
                         callerLineNumber);
        }

        internal static ArgumentException DuplicateQueryParameter(string key,
                                                                  string firstValue,
                                                                  object? secondValue,
                                                                  Assembly callingAssembly,
                                                                  string callerFilePath,
                                                                  string callerMemberName,
                                                                  int callerLineNumber)
        {
            return Build(title: "QUERY PARAMETER SET TWICE",
                         what: [$"{"Key",-10} : {key}", $"{"First",-10} : {Format(firstValue)}", $"{"Second",-10} : {Format(secondValue)}"],
                         why: ["Query keys are matched case-insensitively and set once per chain. Sent twice, the endpoint",
                               "binds only one of the two values - the test would not check what it reads."],
                         fix: ["Set the key once. For several values pass ONE collection (sent as key=a&key=b),",
                               "for another value branch a stored stage:"],
                         fixCode: [".WithQueryParameter(\"" + key + "\", new[] { " + Format(firstValue) + ", " + Format(secondValue) + " })"],
                         callingAssembly,
                         callerFilePath,
                         callerMemberName,
                         callerLineNumber);
        }

        private static ArgumentException Build(string title,
                                               string[] what,
                                               string[] why,
                                               string[] fix,
                                               string[] fixCode,
                                               Assembly callingAssembly,
                                               string callerFilePath,
                                               string callerMemberName,
                                               int callerLineNumber)
        {
            var textDecorator = HttpClientAssertExtensions.GetService<ITextDecorator>(callingAssembly);

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error(Banner));
            sb.AppendLine(textDecorator.Error($"❌ {title}"));
            sb.AppendLine(textDecorator.Error(Banner));
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim(Separator));
            sb.AppendLine();
            sb.AppendLine($"{"Method",-10} : {callerMemberName}");
            sb.AppendLine($"{"Line",-10} : {callerLineNumber}");
            sb.AppendLine($"{"File",-10} : file:///{callerFilePath.Replace('\\', '/')}:{callerLineNumber}");
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("⚠️ What Happened"));
            sb.AppendLine(textDecorator.Dim(Separator));
            sb.AppendLine();

            foreach (var line in what)
            {
                sb.AppendLine(line);
            }

            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("💡 Why This Fails"));
            sb.AppendLine(textDecorator.Dim(Separator));
            sb.AppendLine();

            foreach (var line in why)
            {
                sb.AppendLine(line);
            }

            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("✅ How To Fix"));
            sb.AppendLine(textDecorator.Dim(Separator));
            sb.AppendLine();

            foreach (var line in fix)
            {
                sb.AppendLine(line);
            }

            sb.AppendLine();

            foreach (var line in fixCode)
            {
                sb.AppendLine(textDecorator.Success($"    {line}"));
            }

            sb.AppendLine();

            return new ArgumentException(sb.ToString());
        }

        private static string Format(object? value)
        {
            return value switch
            {
                null => "null",
                string text => $"\"{text}\"",
                _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
            };
        }
    }
}
