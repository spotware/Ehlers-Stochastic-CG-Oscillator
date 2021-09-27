using cAlgo.API;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None), Cloud("CG", "Trigger")]
    public class EhlersStochasticCgOscillator : Indicator
    {
        private IndicatorDataSeries _cg, _v1;

        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 8, MinValue = 1)]
        public int Periods { get; set; }

        [Output("CG", LineColor = "Greeb", PlotType = PlotType.Line)]
        public IndicatorDataSeries Cg { get; set; }

        [Output("Trigger", LineColor = "Red", PlotType = PlotType.Line)]
        public IndicatorDataSeries Trigger { get; set; }

        protected override void Initialize()
        {
            _cg = CreateDataSeries();
            _v1 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            double nm = 0;
            double dm = 0;

            for (int i = 1; i <= Periods; i++)
            {
                nm += (1 + i - 1) * Source[index - i - 1];
                dm += Source[index - i - 1];
            }

            _cg[index] = dm != 0 ? -nm / dm + (Periods + 1) / 2.0 : 0;

            var maxCg = _cg.Maximum(Periods);
            var minCg = _cg.Minimum(Periods);

            _v1[index] = maxCg != minCg ? (_cg[index] - minCg) / (maxCg - minCg) : 0;

            Cg[index] = 2 * (((4 * _v1[index] + 3 * _v1[index - 1] + 2 * _v1[index - 2] + _v1[index - 3]) / 10.0) - 0.5);

            Trigger[index] = 0.96 * (Cg[index - 1] + 0.02);
        }
    }
}