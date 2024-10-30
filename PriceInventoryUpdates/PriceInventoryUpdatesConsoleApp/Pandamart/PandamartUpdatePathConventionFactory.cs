using Application;
using Data.Projections;

namespace PriceInventoryUpdatesConsoleApp
{
    public class CurrentDatePandamartUpdatePathConventionFactory : IPandamartUpdatePathConventionFactory
    {
        private class CurrentDatePandamartUpdatePathConvention : IPandamartUpdatePathConvention
        {
            private readonly string _format;

            public CurrentDatePandamartUpdatePathConvention(string format)
            {
                _format = format;
            }

            public string Localize(Warehouse warehouse, PandamartStore pandamartStore)
            {
                warehouse.Deconstruct(
                    out int code,
                    out string description);

                var currentDate = DateTime.Now;

                return $"{currentDate.ToString(_format)}/{pandamartStore}-{currentDate:yyyyMMdd}-{code}-{description}.csv";
            }
        }

        private readonly string _format;

        public CurrentDatePandamartUpdatePathConventionFactory(string format)
        {
            _format = format;
        }

        public IPandamartUpdatePathConvention Create()
        {
            return new CurrentDatePandamartUpdatePathConvention(format: _format);
        }
    }

    public class PrefixedPandamartUpdatePathConvention : IPandamartUpdatePathConvention
    {
        private readonly string _pathPrefix;

        public PrefixedPandamartUpdatePathConvention(string pathPrefix)
        {
            _pathPrefix = pathPrefix;
        }

        public string Localize(Warehouse warehouse, PandamartStore pandamartStore)
        {
            warehouse.Deconstruct(
                out int code,
                out string description);

            var currentDate = DateTime.Now;

            return $"{_pathPrefix}/{pandamartStore}-{currentDate:yyyyMMdd}-{code}-{description}.csv";
        }
    }

    public class ConsolePromptPandamartUpdatePathConventionFactory : IPandamartUpdatePathConventionFactory
    {
        private readonly string _prompt;

        public ConsolePromptPandamartUpdatePathConventionFactory(string prompt)
        {
            _prompt = prompt;
        }

        public IPandamartUpdatePathConvention Create()
        {
            Console.WriteLine(_prompt);

            return new PrefixedPandamartUpdatePathConvention(pathPrefix: Console.ReadLine() ?? string.Empty);
        }
    }

    public class StaticPrefixValuePandamartUpdatePathConventionFactory : IPandamartUpdatePathConventionFactory
    {
        private readonly PrefixedPandamartUpdatePathConvention _convention;

        public StaticPrefixValuePandamartUpdatePathConventionFactory(string prefix)
        {
            _convention = new PrefixedPandamartUpdatePathConvention(pathPrefix: prefix);
        }

        public IPandamartUpdatePathConvention Create()
        {
            return _convention;
        }
    }

    public class PandamartUpdatePathConventionFactory : IPandamartUpdatePathConventionFactory
    {
        private readonly PrefixedPandamartUpdatePathConvention _convention;

        public PandamartUpdatePathConventionFactory(string prefix)
        {
            _convention = new PrefixedPandamartUpdatePathConvention(pathPrefix: prefix);
        }

        public IPandamartUpdatePathConvention Create()
        {
            return _convention;
        }
    }
}
