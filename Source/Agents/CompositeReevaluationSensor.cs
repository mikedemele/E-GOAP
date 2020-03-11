using System.Collections.Generic;
using System.Linq;

using EGoap.Source.Debug;

namespace EGoap.Source.Agents
{
    // Support for use of multiple reevaluation sensors
    public class CompositeReevaluationSensor : IReevaluationSensor
    {
        private readonly IList<IReevaluationSensor> sensors;

        public CompositeReevaluationSensor(IEnumerable<IReevaluationSensor> sensors = null)
        {
            this.sensors = sensors != null
                ? new List<IReevaluationSensor>(sensors)
                : new List<IReevaluationSensor>();
        }

        #region IReevaluationSensor implementation

        public bool IsReevaluationNeeded
        {
            get { return sensors.Any(sensor => sensor.IsReevaluationNeeded); }
        }

        #endregion

        public void AddSensor(IReevaluationSensor sensor)
        {
            sensors.Add(PreconditionUtils.EnsureNotNull(sensor, nameof(sensor)));
        }

        public void RemoveSensor(IReevaluationSensor sensor)
        {
            sensors.Remove(PreconditionUtils.EnsureNotNull(sensor, nameof(sensor)));
        }
    }
}