using Application;
using Data.Projections;

namespace PriceInventoryUpdatesConsoleApp
{
    public class CurrentDateMetromartUpdatePathConventionFactory : IMetromartUpdatePathConventionFactory
    {
        private class CurrentDateMetromartUpdatePathConvention : IMetromartUpdatePathConvention
        {
            private readonly string _format;

            public CurrentDateMetromartUpdatePathConvention(string format)
            {
                _format = format;
            }

            public string Localize(Warehouse warehouse, MetromartStore metromartStore)
            {
                warehouse.Deconstruct(
                    out int code,
                    out string description);

                var currentDate = DateTime.Now;

                return $"{currentDate.ToString(_format)}/{metromartStore}-{currentDate:yyyyMMdd}-{code}-{description}.csv";
            }
        }

        private readonly string _format;

        public CurrentDateMetromartUpdatePathConventionFactory(string format)
        {
            _format = format;
        }

        public IMetromartUpdatePathConvention Create()
        {
            return new CurrentDateMetromartUpdatePathConvention(format: _format);
        }
    }

    public class PrefixedMetromartUpdatePathConvention : IMetromartUpdatePathConvention
    {
        private readonly string _pathPrefix;

        public PrefixedMetromartUpdatePathConvention(string pathPrefix)
        {
            _pathPrefix = pathPrefix;
        }

        public string Localize(Warehouse warehouse, MetromartStore metromartStore)
        {
            warehouse.Deconstruct(
                out int code,
                out string description);

            var currentDate = DateTime.Now;

            return $"{_pathPrefix}/{metromartStore}-{currentDate:yyyyMMdd}-{code}-{description}.csv";
        }
    }

    public class ConsolePromptMetromartUpdatePathConventionFactory : IMetromartUpdatePathConventionFactory
    {
        private readonly string _prompt;

        public ConsolePromptMetromartUpdatePathConventionFactory(string prompt)
        {
            _prompt = prompt;
        }

        public IMetromartUpdatePathConvention Create()
        {
            Console.WriteLine(_prompt);

            return new PrefixedMetromartUpdatePathConvention(pathPrefix: Console.ReadLine() ?? string.Empty);
        }
    }

    public class StaticPrefixValueMetromartUpdatePathConventionFactory : IMetromartUpdatePathConventionFactory
    {
        private readonly PrefixedMetromartUpdatePathConvention _convention;

        public StaticPrefixValueMetromartUpdatePathConventionFactory(string prefix)
        {
            _convention = new PrefixedMetromartUpdatePathConvention(pathPrefix: prefix);
        }

        public IMetromartUpdatePathConvention Create()
        {
            return _convention;
        }
    }
}
