using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddTextDecoratorProviderExtension
    {
        public static void AddTextDecoratorProvider(this IServiceCollection services)
        {
            services.AddTestSdkSettings();
            services.AddTextRenderStrategies();
            services.AddSingletonIfNotExists<ITextDecoratorProvider, TextDecoratorProvider>();
        }
    }

    /// <summary>
    /// Picks the render strategy for a given consumer assembly.
    ///
    /// <see cref="TestSdkSettings.OutputRendering"/> decides first. Only for
    /// <see cref="OutputRendering.Auto"/> the consumer assembly matters: the sdk ships as a RELEASE
    /// built nuget package, so a <c>#if DEBUG</c> inside it says nothing about the test project
    /// consuming it - it is evaluated when the sdk is compiled. The answer is a property of the
    /// CONSUMER assembly, which is why every caller has to hand it in.
    /// </summary>
    internal interface ITextDecoratorProvider
    {
        /// <summary>
        /// The decorator matching <paramref name="consumerAssembly"/>. A null assembly means the
        /// caller could not be identified and is treated as a CI run - colour stays on under
        /// <see cref="OutputRendering.Auto"/>.
        /// </summary>
        ITextDecorator For(Assembly? consumerAssembly);
    }

    internal sealed class TextDecoratorProvider : ITextDecoratorProvider
    {
        private readonly TestSdkSettings _settings;

        private readonly Dictionary<TextStyle, ITextRenderStrategy> _strategies;

        public TextDecoratorProvider(TestSdkSettings settings,
                                     IEnumerable<ITextRenderStrategy> strategies)
        {
            _settings = settings;

            var registered = strategies.ToList();

            // Same contract as OutputModeRenderer - exactly ONE strategy per style. Checked once here,
            // so a registration conflict fails the first assert instead of picking one at random.
            _strategies = Enum.GetValues<TextStyle>()
                              .ToDictionary(style => style, style => Single(registered, style));
        }

        public ITextDecorator For(Assembly? consumerAssembly)
        {
            // Resolved per call: the settings and an attached debugger may change during a run.
            return _strategies[Resolve(_settings.OutputRendering, consumerAssembly)];
        }

        private static TextStyle Resolve(OutputRendering rendering,
                                         Assembly? consumerAssembly)
        {
            return rendering switch
            {
                OutputRendering.Graphics => TextStyle.Graphics,
                OutputRendering.PlainText => TextStyle.PlainText,
                OutputRendering.Auto => ResolveAuto(consumerAssembly),
                _ => throw new ArgumentOutOfRangeException(nameof(rendering), rendering,
                                                           $"Unknown output rendering. Available: {string.Join(", ", Enum.GetValues<OutputRendering>())}")
            };
        }

        private static TextStyle ResolveAuto(Assembly? consumerAssembly)
        {
            // A debugger suffers the same escape code artifacts as the test explorer does.
            if (Debugger.IsAttached)
            {
                return TextStyle.Monochrome;
            }

            if (consumerAssembly.IsNull())
            {
                return TextStyle.Graphics;
            }

            return consumerAssembly.IsCompiledInDebug() ? TextStyle.Monochrome : TextStyle.Graphics;
        }

        private static ITextRenderStrategy Single(List<ITextRenderStrategy> strategies,
                                                  TextStyle style)
        {
            var matching = strategies.Where(strategy => strategy.CanHandle(style)).ToList();

            if (matching.Count == 0)
            {
                throw new InvalidOperationException($"No text render strategy found for style '{style}'. " +
                                                    "Ensure the appropriate strategy is registered in DI.");
            }

            if (matching.Count > 1)
            {
                throw new InvalidOperationException($"Multiple text render strategies found for style '{style}': " +
                                                    $"{string.Join(", ", matching.Select(strategy => strategy.GetType().Name))}. " +
                                                    "Ensure only ONE strategy can handle each style.");
            }

            return matching[0];
        }
    }
}
