using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace currency_converter
{
    public partial class AppForm : Form
    {
        private const string dataSourceURL = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        private const string dataSourcePath = "eurofxref-daily.xml";
        private CurrencyCalculator calculator = new CurrencyCalculator();
        private string selectedCurrencySymbol;
        private Label oneEquals = new Label();
        private TextBox input = new TextBox();
        private TextBox output = new TextBox();
        private ComboBox currencySelector = new ComboBox();
        public AppForm()
        {
            //setup
            InitializeComponent();

            //get data from file
            try { calculator.GetFromFile(dataSourcePath); }
            catch (Exception)
            {
                MessageBox.Show("File not found! File will be downloaded from server.");
                DownloadFile();
                calculator.GetFromFile(dataSourcePath);
            }
            this.Height = 170;
            this.Width = 350;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            MenuStrip menu = new MenuStrip();
            FlowLayoutPanel ioPanel = new FlowLayoutPanel();
            Label authorLabel = new Label() 
            {
                Text = "Przemysław Lenczewski, przemyslawlenczewski@gmail.com",
                TextAlign = ContentAlignment.BottomCenter,
                Width = Width,
                Dock = DockStyle.Bottom
            };
            Label title = new Label()
            {
                Text = "Currency converter",
                Font = new Font(new FontFamily("Arial"), 16f),
                Width = Width,
                TextAlign = ContentAlignment.TopCenter,
                Top = menu.Bottom
            };
            //menu strip
            ToolStripMenuItem getDataFromServerItem = new ToolStripMenuItem{Text = "Load from URL"};
            ToolStripMenuItem getDataFromFileItem = new ToolStripMenuItem{Text = "Load from file"};
            
            getDataFromServerItem.Click += GetDataFromServer;
            getDataFromFileItem.Click += GetDataFromFile;

            //add items to menu
            menu.Items.AddRange(new ToolStripMenuItem[]{ 
                getDataFromFileItem, 
                getDataFromServerItem 
            });

            //OneEquals label
            oneEquals.Font = new Font(new FontFamily("Arial"), 16f);
            oneEquals.Top = title.Bottom;
            oneEquals.Width = Width;
            oneEquals.TextAlign = ContentAlignment.TopCenter;

            //input box
            input.Name = "input";
            input.TextChanged += OnlyFloatFunction;
            input.TextChanged += Calculate;

            //output box
            output.Name = "output";
            output.TextChanged += OnlyFloatFunction;
            output.TextChanged += Calculate;

            //currency selector
            currencySelector.Name = "currencySelector";
            currencySelector.Items.AddRange(calculator.Currencies.Keys.ToArray());
            currencySelector.SelectedIndexChanged += SelectedCurrencyChanged;
            currencySelector.SelectedIndexChanged += Calculate;
            refreshCurrencySelector();

            //io panel
            ioPanel.Width = Width;
            ioPanel.Top = oneEquals.Bottom;

            //Add stuff to the io panel
            ioPanel.Controls.AddRange(new Control[] {
                input,
                new Label() { 
                    Text = "EUR equals",
                    TextAlign = ContentAlignment.TopCenter,
                },
                output,
                currencySelector
            });

            //Fit stuff in iopanel
            foreach(Control control in ioPanel.Controls)
            {
                control.Width = (Width / ioPanel.Controls.Count)-9;
                control.Font = new Font(new FontFamily("Arial"), 10f);
                control.Height = ioPanel.Height;
            }

            //Add stuff to the window
            this.Controls.AddRange(new Control[]{
                menu,
                authorLabel,
                title,
                oneEquals,
                ioPanel
            });
        }

        private void GetDataFromFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //only xml files
            openFileDialog.Filter = "XML (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                calculator = new CurrencyCalculator();
                try { calculator.GetFromFile(openFileDialog.FileName); }
                catch (Exception)
                {
                    MessageBox.Show("Could not connect to the server!");
                }
                refreshCurrencySelector();
            }
        }

        private void GetDataFromServer(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Type URL address to xml file:", "Load from URL", dataSourceURL);
            if (input.Length.Equals(0)) return;
            calculator = new CurrencyCalculator();
            try { calculator.Connect(input); }
            catch (Exception) {MessageBox.Show("Could not connect to the server!");}
            refreshCurrencySelector();
        }

        private void Calculate(object sender, EventArgs e)
        {
            Control source = (Control)sender;
            if (!source.ContainsFocus) return;
            switch (source.Name)
            {
                case "input":
                    if (input.Text.Length.Equals(0)) return;
                    output.Text = calculator.ConvertFromEur(selectedCurrencySymbol, decimal.Parse(input.Text)).ToString("0.##");
                    break;
                case "output":
                    if (output.Text.Length.Equals(0)) return;
                    input.Text = calculator.ConvertToEur(selectedCurrencySymbol, decimal.Parse(output.Text)).ToString("0.##");
                    break;
                case "currencySelector":
                    if (input.Text.Length.Equals(0) || output.Text.Length.Equals(0)) return;
                    output.Text = calculator.ConvertFromEur(selectedCurrencySymbol, decimal.Parse(input.Text)).ToString("0.##");
                    break;
            };
        }
        private void refreshCurrencySelector()
        {
            currencySelector.Items.Clear();
            currencySelector.Items.AddRange(calculator.Currencies.Keys.ToArray());
            if (!currencySelector.Items.Count.Equals(0))
                currencySelector.SelectedIndex = 0;
        }

        private void SelectedCurrencyChanged(object sender, EventArgs e)
        {
            selectedCurrencySymbol = (string)((ComboBox)sender).SelectedItem;
            oneEquals.Text = "1 EUR equals " + calculator.Currencies[selectedCurrencySymbol] + " " + selectedCurrencySymbol;
        }

        private void OnlyFloatFunction(object sender, EventArgs e)
        {
            TextBox source = (TextBox)sender;
            //if theres nothing in the rextbox, return
            if (source.Text.Length == 0) return;
            source.Text.Replace('.', ',');
            //check if the user typed some non-number characters
            try
            {
                float.Parse(source.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Only numbers are allowed in this field!");
                source.Text = "";
            }
        }
        private void DownloadFile()
        {
            string data = new WebClient().DownloadString(dataSourceURL);
            StreamWriter writer = File.CreateText(dataSourcePath);
            using (writer) writer.Write(data);
            writer.Close();
        }
    }
}
