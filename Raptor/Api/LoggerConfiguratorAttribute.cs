using System;
using System.Reflection;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace Raptor.Api
{
    [AttributeUsage(AttributeTargets.Assembly)]
    internal class LoggerConfiguratorAttribute : ConfiguratorAttribute
    {
        public LoggerConfiguratorAttribute()
            : base(0)
        {
        }

        /// <inheritdoc />
        public override void Configure(Assembly sourceAssembly, ILoggerRepository targetRepository)
        {
            var hierarchy = (Hierarchy)targetRepository;
            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            };
            patternLayout.ActivateOptions();

            var roller = new RollingFileAppender
            {
                AppendToFile = true,
                File = @"logs\raptor.log",
                Layout = patternLayout,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "1GB",
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true
            };
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
        }
    }
}
