
using DiscordRPC;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using Button = DiscordRPC.Button;

namespace discord_status
{
    public partial class window : Form
    {
        public string settingPath = "setting.json";
        public class Setting
        {
            public string? applicationId { get; set; }
            public string? details { get; set; }
            public string? status { get; set; }
            public string? partyMax { get; set; }
            public string? partySize { get; set; }
            public string? largeImageKey { get; set; }
            public string? largeImageText { get; set; }
            public string? smallImageKey { get; set; }
            public string? smallImageText { get; set; }
            public bool timeStampStartCheck { get; set; }
            public bool timeStampEndCheck { get; set; }
            public DateTime timeStampStart { get; set; }
            public DateTime timeStampEnd { get; set; }
            public string? button1Label { get; set; }
            public string? button1URL { get; set; }
            public string? button2Label { get; set; }
            public string? button2URL { get; set; }
        }
        void LogBoxWrite(String logText)
        {

            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    logBox.SelectionStart = logBox.Text.Length;
                    logBox.SelectionLength = 0;
                    if (logBox.Text.Length == 0)
                    {
                        logBox.SelectedText = "[" + System.DateTime.Now.ToString() + "]" + logText;
                    }
                    else
                    {
                        logBox.SelectedText = "\r\n" + "[" + System.DateTime.Now.ToString() + "]" + logText;
                    }
                }));
            }
            else
            {
                logBox.SelectionStart = logBox.Text.Length;
                logBox.SelectionLength = 0;
                if (logBox.Text.Length == 0)
                {
                    logBox.SelectedText = "[" + System.DateTime.Now.ToString() + "]" + logText;
                }
                else
                {
                    logBox.SelectedText = "\r\n" + "[" + System.DateTime.Now.ToString() + "]" + logText;
                }
            }
        }

        void ErrorBoxWrite(String logText)
        {

            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    logBox.SelectionStart = logBox.Text.Length;
                    logBox.SelectionLength = 0;
                    if (logBox.Text.Length == 0)
                    {
                        logBox.SelectedText = "[ERROR]" + "[" + System.DateTime.Now.ToString() + "]" + logText;
                    }
                    else
                    {
                        logBox.SelectedText =  "\r\n" + "[ERROR]" + "[" + System.DateTime.Now.ToString() + "]" + logText;
                    }
                }));
            }
            else
            {
                logBox.SelectionStart = logBox.Text.Length;
                logBox.SelectionLength = 0;
                if (logBox.Text.Length == 0)
                {
                    logBox.SelectedText = "[ERROR]" + "[" + System.DateTime.Now.ToString() + "]" + logText;
                }
                else
                {
                    logBox.SelectedText = "\r\n" + "[ERROR]" + "[" + System.DateTime.Now.ToString() + "]" + logText;
                }
            }
        }

        bool IsUrl(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return Regex.IsMatch(
               input,
               @"^s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+$"
            );
        }
        string ReadAllLine(string filePath, string encodingName)
        {
            StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding(encodingName));
            string allLine = sr.ReadToEnd();
            sr.Close();
            return allLine;
        }

        public DiscordRpcClient client;
        public window()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(settingPath))
                {
                    LogBoxWrite("Loading Setting File...");
                    Setting setting = new Setting();
                    setting = JsonSerializer.Deserialize<Setting>(ReadAllLine(settingPath, "utf-8"));

                    applicationIdText.Text = setting.applicationId;
                    detailsText.Text = setting.details;
                    statusText.Text = setting.status;
                    partyMaxText.Text = setting.partyMax;
                    partySizeText.Text = setting.partySize;
                    largeImageKeyText.Text = setting.largeImageKey;
                    largeImageTextText.Text = setting.largeImageText;
                    smallImageKeyText.Text = setting.smallImageKey;
                    smallImageTextText.Text = setting.smallImageText;
                    timeStampStartCheck.Checked = setting.timeStampStartCheck;
                    timeStampEndCheck.Checked = setting.timeStampEndCheck;
                    timeStampStartText.Value = setting.timeStampStart;
                    timeStampEndText.Value = setting.timeStampEnd;
                    button1LabelText.Text = setting.button1Label;
                    button1URLText.Text = setting.button1URL;
                    button2LabelText.Text = setting.button2Label;
                    button2URLText.Text = setting.button2URL;
                    LogBoxWrite("DONE");
                }
            } catch { }
        }

        private void loginButton(object sender, EventArgs e)
        {
            long applicationId;
            if (!long.TryParse(applicationIdText.Text, out applicationId))
            {
                ErrorBoxWrite("Application ID Invalue");
                MessageBox.Show("�������A�v���P�[�V����ID����͂��Ă��������B",
                    "�G���[ - ApplicationID Invalue",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            if (applicationId < 0)
            {
                ErrorBoxWrite("Application ID Invalue");
                MessageBox.Show("�������A�v���P�[�V����ID����͂��Ă��������B",
                    "�G���[ - ApplicationID Invalue",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            client = new DiscordRpcClient(applicationId.ToString());
            /*
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            */
            client.OnReady += (sender, e) =>
            {
                LogBoxWrite($"Login to Discord with {e.User.Username}");
            };
            client.OnError += (sender, e) =>
            {
                ErrorBoxWrite($" {e}");
            };
            client.OnConnectionFailed += (sender, e) =>
            {
                ErrorBoxWrite($"{e.Type} {e.FailedPipe}");
            };
            client.OnClose += (sender, e) =>
            {
                ErrorBoxWrite($"{e.Code} : {e.Reason}");
                if (e.Code == 4000)
                {
                    MessageBox.Show("�������A�v���P�[�V����ID����͂��Ă��������B",
                        $"�G���[ - {e.Code} : {e.Reason}",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            };
            /*
            client.OnPresenceUpdate += (sender, e) =>
            {
                logBoxWrite($"Received Update! {e.Presence}");
            };*/
            client.Initialize();
        }

        private void setButton(object sender, EventArgs e)
        {
            int partyMax, partySize;
            Party? party = null;
            Button[]? buttons = null;
            Timestamps? timestamp = null;

            if (detailsText.Text.Length == 0)
            {
                MessageBox.Show("�ڍׂ���͂��Ă��������B",
                    "���͂��Ă�������",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (statusText.Text.Length == 0)
            {
                MessageBox.Show("�X�e�[�^�X����͂��Ă��������B",
                    "���͂��Ă�������",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if ((partyMaxText.Text.Length != 0) || (partySizeText.Text.Length != 0)) /*�p�[�e�B�[���̓`�F�b�N*/
            {
                if (!int.TryParse(partyMaxText.Text, out partyMax) || !int.TryParse(partySizeText.Text, out partySize))
                {
                    MessageBox.Show("�������p�[�e�B�[������͂��Ă��������B\n�p�[�e�B�[���͔��p�����݂̂ł��BMax Size �Ƃ��ɓ��͕K�{�ł��B",
                        $"�G���[ - Party Value Invalue",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                if (partyMax < 0 || partySize < 0)
                {
                    MessageBox.Show("�������p�[�e�B�[������͂��Ă��������B\n�p�[�e�B�[���͔��p�����݂̂ł��BMax Size �Ƃ��ɓ��͕K�{�ł��B",
                        $"�G���[ - Party Value Invalue",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                if (partyMax < partySize)
                {
                    MessageBox.Show("�������p�[�e�B�[������͂��Ă��������B\n�p�[�e�B�[Size��Max���͑傫���ł��܂���I",
                        $"�G���[ - Party Value Invalue",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                party = new Party()
                {
                    Size = partySize,
                    Max = partyMax,
                    ID = (new Random().Next(1000, 9999)).ToString(),
                    Privacy = Party.PrivacySetting.Private
                };
            }

            if (timeStampStartCheck.Checked || timeStampEndCheck.Checked)
            {
                if (timeStampStartCheck.Checked)
                {
                    timestamp = new Timestamps()
                    {
                        Start = (timeStampStartText.Value).ToUniversalTime(),
                    };
                }
                else
                {
                    timestamp = new Timestamps()
                    {
                        End = (timeStampEndText.Value).ToUniversalTime()
                    };
                }
            }

            if((button1LabelText.Text.Length != 0) || (button1URLText.Text.Length != 0))
            {
                if (button1LabelText.Text.Length == 0) /*�{�^���̕\�������Ȃ��Ɛݒ�ł��Ȃ�*/
                {
                    MessageBox.Show("�{�^��1��Label����͂��Ă��������B\n�{�^���̕\�������Ȃ��Ɛݒ�ł��܂���B",
                        $"�G���[ - Button1 Value Invalue",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                if (!IsUrl(button1URLText.Text))
                {
                    MessageBox.Show("�{�^��1��URL�ɐ�����URL����͂��Ă��������B\nURL���Ȃ��A�������͊Ԉ����URL�̏ꍇ�ݒ�ł��܂���B",
                        $"�G���[ - Button1 Value Invalue",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                /*button1�ݒ�*/
                buttons = new Button[]
                    {
                        new Button() {
                            Label = button1LabelText.Text,
                            Url = button1URLText.Text
                        }
                    };
            }
            if ((button2LabelText.Text.Length != 0) || (button2URLText.Text.Length != 0))
            {
                if (button1LabelText.Text.Length == 0)
                {
                    MessageBox.Show("�{�^��2�𗘗p����ɂ̓{�^��1��ݒ肵�Ă��������B",
                        $"�G���[ - Button1 Value Invalue",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                if (button2LabelText.Text.Length == 0) /*�{�^���̕\�������Ȃ��Ɛݒ�ł��Ȃ�*/
                {
                    MessageBox.Show("�{�^��2��Label����͂��Ă��������B\n�{�^���̕\�������Ȃ��Ɛݒ�ł��܂���B",
                        $"�G���[ - Button2 Value Invalue",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                if (!IsUrl(button2URLText.Text))
                {
                    MessageBox.Show("�{�^��2��URL�ɐ�����URL����͂��Ă��������B\nURL���Ȃ��A�������͊Ԉ����URL�̏ꍇ�ݒ�ł��܂���B",
                        $"�G���[ - Button1 Value Invalue",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                /*button2�ݒ�*/
                buttons = new Button[]
                    {
                        new Button() {
                            Label = button1LabelText.Text,
                            Url = button1URLText.Text
                        },
                        new Button() {
                            Label = button2LabelText.Text,
                            Url = button2URLText.Text
                        }
                    };
            }

            try {
                client.SetPresence(new RichPresence()
                {
                    Details = detailsText.Text,
                    State = statusText.Text,
                    Assets = new Assets()
                    {
                        LargeImageKey = largeImageKeyText.Text,
                        LargeImageText = largeImageTextText.Text,
                        SmallImageKey = smallImageKeyText.Text,
                        SmallImageText = smallImageTextText.Text
                    },
                    Timestamps = timestamp,
                    Party = party,
                    Buttons = buttons
                });
                LogBoxWrite("Change Discord Status DONE");
                MessageBox.Show("�ύX���܂����I",
                    "Change Discord Status",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            catch(Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                {
                    ErrorBoxWrite("Not logged into Discord : " + ex.Message);
                    MessageBox.Show("Discord�փ��O�C�����Ă���ύX���Ă��������B",
                        "�G���[ - Not logged into Discord",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    MessageBox.Show("�G���[���������܂����BConsole Log�𐻍�҂ɑ��M���Ă��������B",
                        $"�G���[ - {ex.Message}",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    ErrorBoxWrite(ex.Message);
                    return;
                }
            }
        }

        private void timeStampStartCheckChanged(object sender, EventArgs e)
        {
            timeStampEndCheck.Checked = false;
        }
        private void timeStampEndCheckChanged(object sender, EventArgs e)
        {
            timeStampStartCheck.Checked = false;
        }




        private void saveSetting(object sender, EventArgs e)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };
                File.WriteAllText(settingPath, JsonSerializer.Serialize(new Setting()
                {
                    applicationId = applicationIdText.Text,
                    details = detailsText.Text,
                    status = statusText.Text,
                    partyMax = partyMaxText.Text,
                    partySize = partySizeText.Text,
                    largeImageKey = largeImageKeyText.Text,
                    largeImageText = largeImageTextText.Text,
                    smallImageKey = smallImageKeyText.Text,
                    smallImageText = smallImageTextText.Text,
                    timeStampStartCheck = timeStampStartCheck.Checked,
                    timeStampEndCheck = timeStampEndCheck.Checked,
                    timeStampStart = timeStampStartText.Value,
                    timeStampEnd = timeStampEndText.Value,
                    button1Label = button1LabelText.Text,
                    button1URL = button1URLText.Text,
                    button2Label = button2LabelText.Text,
                    button2URL = button2URLText.Text
                }, options));
                LogBoxWrite($"Setting Save DANE - Save Path: {Directory.GetCurrentDirectory()}");
                MessageBox.Show("�ۑ����܂����I",
                    "Setting Save DANE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            catch(Exception ex)
            {
                MessageBox.Show("�G���[���������܂����BConsole Log�𐻍�҂ɑ��M���Ă��������B",
                    $"�G���[ - {ex.Message}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                ErrorBoxWrite(ex.Message);
                return;
            }
        }

        private void exportSetting(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "�ݒ���G�N�X�|�[�g";
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.FileName = settingPath;
            saveFileDialog.Filter = "JSON File(*.json)|*.json";
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.OverwritePrompt = true;
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        WriteIndented = true
                    };
                    File.WriteAllText(saveFileDialog.FileName, JsonSerializer.Serialize(new Setting()
                    {
                        applicationId = applicationIdText.Text,
                        details = detailsText.Text,
                        status = statusText.Text,
                        partyMax = partyMaxText.Text,
                        partySize = partySizeText.Text,
                        largeImageKey = largeImageKeyText.Text,
                        largeImageText = largeImageTextText.Text,
                        smallImageKey = smallImageKeyText.Text,
                        smallImageText = smallImageTextText.Text,
                        timeStampStartCheck = timeStampStartCheck.Checked,
                        timeStampEndCheck = timeStampEndCheck.Checked,
                        timeStampStart = timeStampStartText.Value,
                        timeStampEnd = timeStampEndText.Value,
                        button1Label = button1LabelText.Text,
                        button1URL = button1URLText.Text,
                        button2Label = button2LabelText.Text,
                        button2URL = button2URLText.Text
                    }, options));
                    LogBoxWrite($"Setting Export DANE - Export Path: {saveFileDialog.FileName}");
                    MessageBox.Show($"�ݒ���G�N�X�|�[�g���܂����I\n�G�N�X�|�[�g��: {saveFileDialog.FileName}",
                        "Setting Export DANE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("�G���[���������܂����BConsole Log�𐻍�҂ɑ��M���Ă��������B",
                        $"�G���[ - {ex.Message}",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    ErrorBoxWrite(ex.Message);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                return;
            }
        }

        private void importSetting(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "�ݒ���C���|�[�g";
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.FileName = settingPath;
            openFileDialog.Filter = "JSON File(*.json)|*.json";
            openFileDialog.FilterIndex = 1;
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    LogBoxWrite("Import Setting File...");
                    Setting setting = new Setting();
                    setting = JsonSerializer.Deserialize<Setting>(ReadAllLine(openFileDialog.FileName, "utf-8"));

                    applicationIdText.Text = setting.applicationId;
                    detailsText.Text = setting.details;
                    statusText.Text = setting.status;
                    partyMaxText.Text = setting.partyMax;
                    partySizeText.Text = setting.partySize;
                    largeImageKeyText.Text = setting.largeImageKey;
                    largeImageTextText.Text = setting.largeImageText;
                    smallImageKeyText.Text = setting.smallImageKey;
                    smallImageTextText.Text = setting.smallImageText;
                    timeStampStartCheck.Checked = setting.timeStampStartCheck;
                    timeStampEndCheck.Checked = setting.timeStampEndCheck;
                    timeStampStartText.Value = setting.timeStampStart;
                    timeStampEndText.Value = setting.timeStampEnd;
                    button1LabelText.Text = setting.button1Label;
                    button1URLText.Text = setting.button1URL;
                    button2LabelText.Text = setting.button2Label;
                    button2URLText.Text = setting.button2URL;
                    LogBoxWrite("DANE");
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("�G���[���������܂����BConsole Log�𐻍�҂ɑ��M���Ă��������B",
                        $"�G���[ - {ex.Message}",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    ErrorBoxWrite(ex.Message);
                    return;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                return;
            }
        }

        private void resetSetting(object sender, EventArgs e)
        {
            applicationIdText.Text = "";
            detailsText.Text = "";
            statusText.Text = "";
            partyMaxText.Text = "";
            partySizeText.Text = "";
            largeImageKeyText.Text = "";
            largeImageTextText.Text = "";
            smallImageKeyText.Text = "";
            smallImageTextText.Text = "";
            timeStampStartCheck.Checked = false;
            timeStampEndCheck.Checked = false;
            timeStampStartText.Value = DateTime.Now;
            timeStampEndText.Value = DateTime.Now;
            button1LabelText.Text = "";
            button1URLText.Text = "";
            button2LabelText.Text = "";
            button2URLText.Text = "";
        }

        private void kuwacom_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://kuwa.dev/",
                UseShellExecute = true,
            });
        }

        private void kuwa_network_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://kuwa.app/",
                UseShellExecute = true,
            });

        }
    }
}