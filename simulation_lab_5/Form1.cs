using System;
using System.Text;
using System.Windows.Forms;

namespace simulation_lab_5
{
    public partial class Form1 : Form
    {
        private readonly double[][] _transitionRateMatrix;

        public Form1()
        {
            InitializeComponent();

            _transitionRateMatrix = new double[][]
            {
                new double[] { -0.4, 0.3, 0.1 },
                new double[] { 0.4, -0.8, 0.4 },
                new double[] { 0.1, 0.4, -0.5 },
            };

            dataGridView1.Rows.Add();
            dataGridView1.Rows[0].HeaderCell.Value = "Frequency";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedWeatherState = comboBox1.SelectedIndex;
            if (selectedWeatherState == -1)
            {
                MessageBox.Show("Select weather state");

                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Enter number of days");

                return;
            }

            if (textBox1.Text == "0")
            {
                MessageBox.Show("Enter valid number of days");

                return;
            }

            var numberOfDays = Convert.ToInt32(textBox1.Text);

            var continuousMarkovProcessGenerator = new ContinuousMarkovProcessGenerator(_transitionRateMatrix, (WeatherState)selectedWeatherState);
            var weatherLog = continuousMarkovProcessGenerator.SimulateAndGetInfo(numberOfDays);

            var sb = new StringBuilder();

            for (var i = 0; i < weatherLog.Count; i++)
            {
                var timeSpan = TimeSpan.FromDays(weatherLog[i].Item1 + 1);
                string weatherState = string.Empty;
                switch (weatherLog[i].Item2)
                {
                    case (int)WeatherState.Clear:
                        weatherState = "Clear";
                        break;
                    case (int)WeatherState.Cloudy:
                        weatherState = "Cloudy";
                        break;
                    case (int)WeatherState.Overcast:
                        weatherState = "Overcast";
                        break;
                }

                sb.Append($"Day: {timeSpan.Days} hour: {timeSpan.Hours}:00, weather: {weatherState}");
                sb.Append(Environment.NewLine);

                // last time period
                if (i == weatherLog.Count - 1)
                {
                    timeSpan = TimeSpan.FromDays(numberOfDays + 1);
                    sb.Append($"Day: {timeSpan.Days} hour: {timeSpan.Hours}:00, weather: {weatherState}");
                    sb.Append(Environment.NewLine);
                }
            }

            var weatherLogString = sb.ToString();

            textBox2.Text = weatherLogString;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsDigit(number) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsDigit(number) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!char.IsDigit(number) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Enter sample size");

                return;
            }

            if (textBox3.Text == "0")
            {
                MessageBox.Show("Enter valid sample size");

                return;
            }

            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Enter time shift");

                return;
            }

            if (textBox4.Text == "0")
            {
                MessageBox.Show("Enter valid time shift");

                return;
            }

            var sampleSize = Convert.ToInt32(textBox3.Text);
            var timeShift = Convert.ToInt32(textBox4.Text);

            var continuousMarkovProcessGenerator = new ContinuousMarkovProcessGenerator(_transitionRateMatrix, WeatherState.Clear);
            var statistics = continuousMarkovProcessGenerator.GetStatisticalProcessingInfo(sampleSize, timeShift);

            for (var i = 0; i < statistics.Count; i++)
            {
                dataGridView1.Rows[0].Cells[i].Value = statistics[i];
            }
        }
    }
}
