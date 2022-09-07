using AVerCapSDKDemo;
using Criteria;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml;
using System.Timers;
using System.Diagnostics;

namespace VGACheck
{
    public partial class Index : Form
    {
        private delegate void delegateStatustext(string value);
        private delegate void delegateLabelKvm(string value);
        private delegate void delegateLabelClock();
        private delegate void delegateTestingStart(bool b);
        private delegate void delegateErrorCheck(bool b);
        private delegate void delegateKvmChange(int i);

        private IntPtr m_hCaptureDevice = new IntPtr();
        public uint m_uCaptureType = 0;
        public int m_iCurrentDeviceIndex = -1;
        string data_string = string.Empty, path_run, path_tmp, path_pass, path_log_pass, path_log, status_string, path_date, path_time; //暫存字串
        int data_int = 0;
        Point point_checking, point_kvm, point_clock;
        bool bool_checking, bool_hide = false, bool_full = false;

        public Index()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void Index_Load(object sender, EventArgs e) //畫面開啟
        {
            //Criteriastr.argv = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".xml";
            label_kvmnum.Text = string.Empty;
            //ini 
            if (Read_Screen())
            {
                Statustext($"{Criteriastr.argv} format pass!");
                combo_port.Enabled = true;
                text_baud.Enabled = true;
                button_main.Enabled = true;
                button_mac.Enabled = true;
                button_image_interval.Enabled = true;
                button_image_range.Enabled = true;
            }
            else
            {
                Statustext($"{Criteriastr.argv} format fail!");
                this.BackColor = Color.Red;
                return;
            }
            this.Text = $"{Criteriastr.title} - {Criteriastr.ver}";
            group_xml.Text = Criteriastr.argv;

            if (AVerCapAPI_connect())
            {
                Statustext("Capture card connect successful!");
                button_vga.Enabled = true;
                combo_rel.Enabled = true;
            }
            else
            {
                this.BackColor = Color.Red;
                return;
            }

            //ini [End]
            text_baud.Text = Criteriastr.baud;
            for (int i = 0; i < Criteriaint.fix_count; i++)
            {
                combo_kvm.Items.Add($"{i + 1}. {Criteriastr.fix_num[i]}");
            }
            //Get all port
            string[] ports_name = System.IO.Ports.SerialPort.GetPortNames();
            combo_port.Items.AddRange(ports_name);
            //Get all port [End]
            button_kvm.Enabled = text_baud.Text != "" && combo_port.Text != "";

            //Make listview
            point_checking = label_checking.Location;
            point_kvm = combo_kvm.Location;
            point_clock = label_clock.Location;
            //Make listview [End]
        }

        private void Index_closed(object sender, FormClosedEventArgs e) //畫面關閉
        {
            if (Criteriabool.vgarun) AVerCapAPI.AVerStopStreaming(m_hCaptureDevice);
            if (Criteriabool.autorun) bw_auto.CancelAsync();
            if (serial_kvm.IsOpen) serial_kvm.Close();
        }

        private void Index_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to close？", "Closing Program", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private bool AVerCapAPI_connect()
        {
            int iTempdeviceType = (int)CAPTURETYPE.CAPTURETYPE_SD;
            //int iTempdeviceType = (int)CAPTURETYPE.CAPTURETYPE_HD;
            int iTempIndex = 0;
            m_uCaptureType = 0;
            m_iCurrentDeviceIndex = -1;
            int iRetVal = 0;
            try
            {
                iRetVal = AVerCapAPI.AVerCreateCaptureObjectEx((uint)iTempIndex, (uint)iTempdeviceType, pictureBoxShowMain.Handle, ref m_hCaptureDevice);
            }
            catch (Exception ex)
            {
                Statustext(
                    "<ERROR> Capture card connect failed!\r\n" +
                    "\tPlease check the driver install.\r\n" +
                   $"\t({ex.Message})"
                );
                return false;
            }
            switch (iRetVal)
            {
                case (int)ERRORCODE.CAP_EC_SUCCESS:
                    break;
                case (int)ERRORCODE.CAP_EC_DEVICE_IN_USE:
                    Statustext("<ERROR> Capture card is used by other program!");
                    return false;
                default:
                    Statustext("<ERROR> Capture card can not initialization!");
                    return false;
            }
            m_iCurrentDeviceIndex = iTempIndex;
            if (m_uCaptureType == 0)
            {
                m_uCaptureType = (uint)CAPTURETYPE.CAPTURETYPE_HD;
            }
            ///PAL
            AVerCapAPI.AVerSetVideoFormat(m_hCaptureDevice, (uint)VIDEOFORMAT.VIDEOFORMAT_PAL);

            bool support_rel = true;
            uint m_uVideoSource = 0;
            uint m_uVideoFormat = 0;
            uint SolutionNum = 0;
            AVerCapAPI.AVerGetVideoSource(m_hCaptureDevice, ref m_uVideoSource);
            AVerCapAPI.AVerGetVideoFormat(m_hCaptureDevice, ref m_uVideoFormat);
            AVerCapAPI.AVerGetVideoResolutionSupported(m_hCaptureDevice, m_uVideoSource, m_uVideoFormat, null, ref SolutionNum);
            uint[] pdwSupported = new uint[SolutionNum];
            AVerCapAPI.AVerGetVideoResolutionSupported(m_hCaptureDevice, m_uVideoSource, m_uVideoFormat, pdwSupported, ref SolutionNum);
            Criteriaint.resolutaion_list = new uint[pdwSupported.Length];
            Criteriaint.resolutaion_num = pdwSupported[0];
            data_string = string.Empty;
            for (int i = 0; i < pdwSupported.Length; i++)
            {
                combo_rel.Items.Add(Screen_resolutaion(pdwSupported[i]));
                Criteriaint.resolutaion_list[i] = pdwSupported[i];
                if(Criteriastr.resolutaion == Screen_resolutaion(pdwSupported[i]) && support_rel)
                {
                    Criteriaint.resolutaion_num = pdwSupported[i];
                    support_rel = false;
                }
                data_string += $"{Screen_resolutaion(pdwSupported[i])}, ";
            }
            combo_rel.Text = Screen_resolutaion(Criteriaint.resolutaion_num);
            if (support_rel && Criteriastr.resolutaion != string.Empty)
            {
                Statustext($"<ERROR> Cannot support resolutaion : {Criteriastr.resolutaion}\r\n\tSupport Resolutaion : {data_string}");
                return false;
            }
            return true;
        }

        private void combo_rel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Criteriaint.resolutaion_num = Criteriaint.resolutaion_list[combo_rel.Items.IndexOf(combo_rel.Text)];
            //pictureBoxShowMain.Height = (int)(pictureBoxShowMain.Width * double.Parse(combo_rel.Text.Split('x')[1]) / double.Parse(combo_rel.Text.Split('x')[0]));
        }

        private void button_vga_Click(object sender, EventArgs e) //VGA連接按鈕
        {
           
            if (Criteriabool.vgarun)
            {
                AVerCapAPI.AVerStopStreaming(m_hCaptureDevice);

            }
            else
            {
                ///解析度
                VIDEO_RESOLUTION VideoResolution;
                uint m_uVideoResolution = 0;
                VideoResolution = new VIDEO_RESOLUTION();
                VideoResolution.dwVersion = 1;
                AVerCapAPI.AVerGetVideoResolutionEx(m_hCaptureDevice, ref VideoResolution);
                m_uVideoResolution = VideoResolution.dwVideoResolution;
                VideoResolution.dwVersion = 1;
                VideoResolution.bCustom = 0;
                VideoResolution.dwVideoResolution = Criteriaint.resolutaion_num;
                AVerCapAPI.AVerSetVideoResolutionEx(m_hCaptureDevice, ref VideoResolution);
                Criteriastr.resolutaion = Screen_resolutaion(Criteriaint.resolutaion_num);

                ///show出畫面
                AVerCapAPI.AVerStartStreaming(m_hCaptureDevice);
                SetVideoWindow();
                //30s
                button_vga.Enabled = false;
                textBox_timer.Visible = true;
                timer.Interval = 1000;
                timer.Start();
                /*SetTimer();
                aTimer.Start();
                aTimer.Stop();
                aTimer.Dispose();*/
            }
            Criteriabool.vgarun = !Criteriabool.vgarun;
            combo_rel.Enabled = !Criteriabool.vgarun;
            label_vga.Text = Criteriabool.vgarun ? "VGA ON" : "VGA OFF";
            label_vga.ForeColor = Criteriabool.vgarun ? Color.Green : Color.Black;
            button_vga.Image = Criteriabool.vgarun ? Properties.Resources.btncheckon : Properties.Resources.btncheckoff;
            label_auto.ForeColor = (Criteriabool.kvmerror) ? Color.Black : Color.Gray;
            button_auto.Enabled = true;
            button_kvm.Enabled = true;
            button_screen.Enabled = Criteriabool.vgarun && serial_kvm.IsOpen;
            Statustext(Criteriabool.vgarun ? $"VGA : ON ({Criteriastr.resolutaion})" : "VGA : OFF");

        }

        private static System.Timers.Timer aTimer;


        private static void SetTimer()
        { 
            aTimer = new System.Timers.Timer(30000); 
            aTimer.AutoReset = true;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
        }

        public static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
           
        }
        private void SetVideoWindow()
        {
            RECT RectClient = new RECT();
            RectClient.Left = 0;
            RectClient.Top = 0;
            RectClient.Right = pictureBoxShowMain.Width;
            RectClient.Bottom = pictureBoxShowMain.Height;
            AVerCapAPI.AVerSetVideoWindowPosition(m_hCaptureDevice, RectClient);
        }

        private void button_kvm_Click(object sender, EventArgs e) //PLC連接按鈕
        {
            if (Criteriabool.kvmerror)
            {
                Statustext($"KVM PLC Error!");
                Criteriabool.kvmerror = false;
            }
            else if (serial_kvm.IsOpen)
            {
                serial_kvm.Close();
            }
            else
            {
                try
                {
                   serial_kvm.BaudRate = int.Parse(text_baud.Text);
                }
                catch
                {
                    Statustext($"<ERROR> Baud not number! (BaudText = [{text_baud.Text}])"); 
                    return;
                    }
                    serial_kvm.PortName = combo_port.Text;
                    try
                    {
                        serial_kvm.Open();
                    }
                    catch (Exception ex)
                    {
                        Statustext(
                            $"<ERROR> KVM connect failed!\r\n" +
                            $"\t{ex.Message}"
                        );
                        return;
                    }
                if (combo_kvm.SelectedIndex == -1)
                {
                    Criteriaint.kvm = 0;
                    combo_kvm.SelectedIndex = Criteriaint.kvm;
                }
                this.BackColor = SystemColors.Control;
                Criteriabool.kvmerror = false;
            }
            label_kvm.Text = serial_kvm.IsOpen ? "KVM ON" : "KVM OFF";
            label_kvm.ForeColor = serial_kvm.IsOpen ? Color.Green : Color.Black;
            text_baud.Enabled = !serial_kvm.IsOpen;
            combo_port.Enabled = !serial_kvm.IsOpen;
            button_kvm.Image = serial_kvm.IsOpen ? Properties.Resources.btncheckon : Properties.Resources.btncheckoff;
            label_auto.ForeColor = (Criteriabool.vgarun && serial_kvm.IsOpen) ? Color.Black : Color.Gray;
            button_auto.Enabled = Criteriabool.vgarun && serial_kvm.IsOpen;
            button_screen.Enabled = Criteriabool.vgarun && serial_kvm.IsOpen;
            combo_kvm.Enabled = serial_kvm.IsOpen;
            Statustext(serial_kvm.IsOpen ? $"KVM PLC : ON" : "KVM PLC : OFF");
        }

        private void text_baud_TextChanged(object sender, EventArgs e) //輸入baud
        {
            button_kvm.Enabled = text_baud.Text != "" && combo_port.Text != "";
        }

        private void button_main_Click(object sender, EventArgs e) //title, backup, shopfloor, baud, wait 資訊
        {
            data_string =
                $"<Main Information>\r\n" +
                $"\tVersion : {Criteriastr.ver}\r\n" +
                $"\tBackup Path : {Criteriastr.backup}\r\n" +
                $"\tShopfloor Path : {Criteriastr.shopfloor}\r\n" +
                $"\tBaud : {Criteriastr.baud}\r\n" +
                $"\tWait time : {Criteriaint.wait} seconds\r\n" +
                $"\tMessage Word Limit : {Criteriaint.status_max}\r\n";
            Statustext(data_string);
        }

        private void button_mac_Click(object sender, EventArgs e) // mac 詳細資訊
        {
            data_string = "<Fxiture Information>\r\n";
            for (int i = 0; i < Criteriaint.fix_count; i++)
            {
                data_string += $"\t{i + 1}. [{Criteriastr.fix_num[i]}]\r\n";
            }
            Statustext(data_string);
        }

        private void button_image_range_Click(object sender, EventArgs e) //image check 像素範圍
        {
            data_string =
                "<Image Check Range>\r\n" +
                "\t[Image ID] : [Start X, Start Y] ~ [End X, End Y]\r\n";
            for (int i = 0; i < Criteriaint.image_count; i++)
            {
                data_string += $"\t{Criteriastr.image_id[i]} Range : [{Criteriaint.range[i, 0]} , {Criteriaint.range[i, 1]}] ~ [{Criteriaint.range[i, 2]} , {Criteriaint.range[i, 3]}]\r\n";
            }
            Statustext(data_string);
        }

        private void button_image_interval_Click(object sender, EventArgs e) //image check 色相檢查範圍
        {
            string[] item_string = { "AVER", "MAX", "MIN" };
            data_string =
                "<Image Check Color>\r\n" +
                "\t[ImageID] [Item] : [Red] [Greeb] [Blue]\r\n";
            for (int i = 0; i < Criteriaint.image_count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    data_string += $"\t{Criteriastr.image_id[i]} {item_string[j]} : [{Criteriaint.interval[i, j, 0]}~{Criteriaint.interval[i, j, 1]}] , [{Criteriaint.interval[i, j, 2]}~{Criteriaint.interval[i, j, 3]}] , [{Criteriaint.interval[i, j, 4]}~{Criteriaint.interval[i, j, 5]}]\r\n";
                }
            }
            Statustext(data_string);
        }

        private void combo_port_SelectedIndexChanged(object sender, EventArgs e) //選擇port
        {
            button_kvm.Enabled = text_baud.Text != "" && combo_port.Text != "";
        }

        private void button_auto_Click(object sender, EventArgs e) //自動化開啟按鈕
        {
            if (Criteriabool.autorun)
            {
                if (!bw_auto.WorkerSupportsCancellation)
                {
                    return;
                }
                bw_auto.CancelAsync();
                Statustext("Automation : OFF");
                label_clock.Text = "Automation OFF";
            }
            else
            {
                if (!Directory.Exists(Criteriastr.shopfloor))
                {
                    Statustext($"<ERROR> Can't find shopfloor\r\n\t{Criteriastr.shopfloor}");
                    return;
                }
                if (!Directory.Exists(Criteriastr.backup))
                {
                    Statustext($"<ERROR> Can't find backup\r\n\t{Criteriastr.backup}");
                    return;
                }
                if (!serial_kvm.IsOpen)
                {
                    Criteriabool.kvmerror = true;
                    button_kvm.PerformClick();
                    return;
                }
                try
                {
                    bw_auto.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                    Statustext(
                        "<ERROR> Automation failed!\r\n" +
                        $"\t{ex.Message}"
                    );
                    return;
                }

                Statustext("Automation : ON");
                button_auto.Visible = false;
                //combo_kvm.SelectedIndex = -1;
                this.BackColor = SystemColors.Control;
            }
            Criteriabool.autorun = !Criteriabool.autorun;
            label_auto.Text = Criteriabool.autorun ? "Automation ON" : "Automation OFF";
            label_auto.ForeColor = Criteriabool.autorun ? Color.Green : Color.Black;
            button_auto.Image = Criteriabool.autorun ? Properties.Resources.btncheckon : Properties.Resources.btncheckoff;
            //button_vga.Enabled = !Criteriabool.autorun;
            button_kvm.Enabled = !Criteriabool.autorun;
            button_screen.Enabled = !Criteriabool.autorun;
            //combo_kvm.Enabled = !Criteriabool.autorun;
            this.BackColor = Criteriabool.autorun ? Color.PaleTurquoise : SystemColors.Window;
            this.Text = Criteriabool.autorun ? $"{Criteriastr.title} - Automation running..." : $"{Criteriastr.title} - {Criteriastr.ver}";
            bool_checking = false;
            button_auto.Visible = false;
        }

        private void button_screen_Click(object sender, EventArgs e) //手動擷取圖片按鈕
        {
            if (!Directory.Exists(Criteriastr.backup))
            {
                Statustext($"<ERROR> Can't find backup\r\n\t{Criteriastr.backup}");
                return;
            }
            button_screen.Enabled = false;
            button_vga.Enabled = false;
            button_kvm.Enabled = false;
            button_auto.Enabled = false;
            bw_manual.RunWorkerAsync();
            combo_kvm.Enabled = false;
        }

        private void bw_manual_DoWork(object sender, DoWorkEventArgs e) //手動確認
        {
            path_date = DateTime.Now.ToString("yyyyMMdd");
            path_time = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (!Directory.Exists(Path.Combine(Criteriastr.backup, path_date)))
            {
                Directory.CreateDirectory(Path.Combine(Criteriastr.backup, path_date));
                Thread.Sleep(50);
            }
            Image_Check("Manual");
        }

        private void bw_manual_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            File.WriteAllText(Path.Combine(Criteriastr.backup, path_date, $"{Criteriastr.fix_num[Criteriaint.kvm]}_Manual_{path_time}.log"), Criteriastr.log);
            button_screen.Enabled = true;
            button_vga.Enabled = true;
            button_kvm.Enabled = true;
            button_auto.Enabled = true;
            combo_kvm.Enabled = true;
        }

        private void combo_kvm_SelectedIndexChanged(object sender, EventArgs e) //手動選擇kvm
        {
            if (!serial_kvm.IsOpen)
            {
                if (pictureBoxShowMain.Dock == DockStyle.Fill) ScreenChange();
                button_kvm.PerformClick();
                this.BackColor = Color.Red;
                Statustext("<ERROR> KVM disconnect!");
                Criteriabool.kvmerror = true;
                return;
            }
            int i = combo_kvm.SelectedIndex;
            if (i < 0 || i > Criteriaint.fix_count - 1) return;
            Criteriaint.kvm = i;
            label_kvmnum.Text = $"Now Fixture : {Criteriaint.kvm + 1}. {Criteriastr.fix_num[Criteriaint.kvm]}";
            Send_PLC(Criteriastr.fix_send[i]);
        }

        private void bw_auto_DoWork(object sender, DoWorkEventArgs e) //自動化循環
        {
            string sn;
            // checkimage : 回傳影像判斷結果
            // sn : .run裡的sn資訊
            while (!bw_auto.CancellationPending) //後台執行時跑迴圈。反之中斷
            {
                ClockRunning();
                if (!Directory.Exists(Criteriastr.shopfloor))
                {
                    Statustext("<ERROR> Can't find shopfloor path!");
                    AutoTestError(true);
                    Thread.Sleep(1000);
                    FolderReConnect(Criteriastr.shopfloor);
                }
                if (!Directory.Exists(Criteriastr.backup))
                {
                    Statustext("<ERROR> Can't find backup path!");
                    AutoTestError(true);
                    Thread.Sleep(1000);
                    FolderReConnect(Criteriastr.backup);
                }
                if (!serial_kvm.IsOpen)
                {
                    Criteriabool.kvmerror = true;
                    Statustext("<ERROR> KVM PLC disconnect!");
                    AutoTestError(true);
                    Thread.Sleep(1000);
                    KVMReConnect();
                }
                path_date = DateTime.Now.ToString("yyyyMMdd");
                path_time = DateTime.Now.ToString("yyyyMMddHHmmss");
                for (int fix_while = 0; fix_while < Criteriaint.fix_count; fix_while++)
                {
                    if (Directory.GetFiles(Criteriastr.shopfloor, $"{Criteriastr.fix_num[fix_while]}.run").Length != 0) //是否有收到對應的{fix}.run
                    {
                        path_run = Path.Combine(Criteriastr.shopfloor, $"{Criteriastr.fix_num[fix_while]}.run");
                        path_tmp = Path.Combine(Criteriastr.shopfloor, $"{Criteriastr.fix_num[fix_while]}.tmp");
                        //path_pass = Path.Combine(Criteriastr.shopfloor, $"{Criteriastr.fix_num[fix_while]}.pass");
                        sn = File.ReadAllText(path_run).Trim();
                        if (sn != string.Empty) //檢查.run 是否有SN
                        {
                            
                            //終止手動，切換自動
                            if (!Criteriabool.vgarun) //判斷VGA button 是否開啟，如果沒有 開啟
                            {
                                button_vga.PerformClick();
                                Thread.Sleep(1000);
                            }
                            timer.Stop();
                            second = 30;

                            if (!Directory.Exists(Path.Combine(Criteriastr.backup, path_date)))
                            {
                                Directory.CreateDirectory(Path.Combine(Criteriastr.backup, path_date));
                                Thread.Sleep(50);
                            }
                            Criteriaint.kvm = fix_while;
                            bool_checking = true;
                            SelectKVM(Criteriaint.kvm);
                            Thread.Sleep(50);
                            TestingStart(true);
                            //@ PLC切換 [Start]
                            Send_PLC(Criteriastr.fix_send[Criteriaint.kvm]); //切換kvm (again)
                            Statustext($"< {Criteriastr.fix_num[Criteriaint.kvm]} > {sn} Get : Wait {Criteriaint.wait} seconds..."); //等待
                            path_log = Path.Combine(Criteriastr.backup, path_date, $"{Criteriastr.fix_num[Criteriaint.kvm]}_{sn}_{path_time}_ori.log");//original log
                            path_log_pass = Path.Combine(Criteriastr.backup, path_date, $"{Criteriastr.fix_num[Criteriaint.kvm]}_{sn}_{path_time}.log");//pass log          

                            Thread.Sleep(Criteriaint.wait * 1000);
                            //@ PLC切換 [End]
                            Image_Check(sn); //Check Image
                            File.WriteAllText(path_log, Criteriastr.log); //寫入log
                            File.WriteAllText(path_log_pass, Criteriastr.log); //寫入pass log
                            File.WriteAllText(path_log_pass, File.ReadAllText(path_log_pass).Replace("FAIL", "PASS"));
                            if (File.Exists(path_run)) File.Delete(path_run); //刪除舊run
                            if (File.Exists(path_tmp)) File.Delete(path_tmp); //刪除舊tmp
                            //if (File.Exists(path_pass)) File.Delete(path_pass); //刪除舊pass
                            File.Copy(path_log_pass, path_tmp, true); //給shopfloor pass tmp
                            //File.Copy(path_log_pass, path_pass, true); //給shopfloor pass log
                            
                            //自動結束，歸還手動
                            bool_checking = false;
                            TestingStart(false);
                            if (Criteriabool.vgarun) //判斷VGA button 是否開啟，如果有 關閉
                            {
                                
                                textBox_timer.Visible = false;                             
                                button_vga.Enabled = true;
                                Criteriabool.testdetected = false;
                                Thread.Sleep(1000);
                                button_vga.PerformClick();                               
                            }
                            break;
                        }
                        else //.run 沒有SN
                        {
                            Statustext($"< {Criteriastr.fix_num[fix_while]} > NO SN !");
                            path_log = Path.Combine(Criteriastr.backup, $"{Criteriastr.fix_num[fix_while]}_ERROR.log");
                            path_log_pass = Path.Combine(Criteriastr.backup, path_date, $"{Criteriastr.fix_num[Criteriaint.kvm]}_{sn}_{path_time}.log");
                            Criteriastr.log =
                                $"RESULT=FAIL;\r\n" +
                                $"SN=;\r\n" +
                                $"Error_Msg=Not found sn in {Criteriastr.fix_num[fix_while]}.run;"
                            ;
                            File.WriteAllText(path_log, Criteriastr.log); //寫入log
                            File.WriteAllText(path_log_pass, Criteriastr.log); //寫入pass log
                            File.WriteAllText(path_log_pass, File.ReadAllText(path_log_pass).Replace("FAIL", "PASS"));
                            if (File.Exists(path_run)) File.Delete(path_run); //刪除舊run
                            if (File.Exists(path_tmp)) File.Delete(path_tmp); //刪除舊tmp
                            //if (File.Exists(path_pass)) File.Delete(path_pass); //刪除舊pass
                            File.Copy(path_log_pass, path_tmp, true); //給shopfloor 新tmp
                            //File.Copy(path_log_pass, path_pass, true); //給shopfloor 新pass
                            break;
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void bw_auto_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) //網路異常處理
        {
            if (Criteriabool.autoerror)
            {
                bw_auto.CancelAsync();
                Criteriabool.autoerror = false;
                Criteriabool.autorun = false;
                label_auto.Text = "Automation OFF";
                label_auto.ForeColor = Color.Black;
                button_auto.Image = Properties.Resources.btncheckoff;
                button_vga.Enabled = false;
                button_kvm.Enabled = false;
                button_screen.Enabled = false;
                combo_kvm.Enabled = false;
                Statustext("<ERROR> Automation break off!");
                //label_status.ForeColor = Color.Red;
                MessageBox.Show(data_string, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Image_Check(string sn) //image 檢測
        {
            string image_address = Path.Combine(Criteriastr.backup, path_date, $"{Criteriastr.fix_num[Criteriaint.kvm]}_{sn}_{path_time}.png");

            //@ 截圖 [Start]
            if (File.Exists("Result.png")) File.Delete("Result.png"); //若路徑上有該圖片，先把該圖片刪除
            CAPTURE_IMAGE_INFO m_CaptureImageInfo = new CAPTURE_IMAGE_INFO();
            m_CaptureImageInfo.dwCapNumPerSec = 0;
            m_CaptureImageInfo.lpFileName = "Result.png";
            m_CaptureImageInfo.dwImageType = (uint)IMAGETYPE.IMAGETYPE_PNG;
            m_CaptureImageInfo.dwCaptureType = (uint)CT_SEQUENCE.CT_SEQUENCE_FRAME;
            m_CaptureImageInfo.bOverlayMix = 1;
            m_CaptureImageInfo.rcCapRect.Left = 0;
            m_CaptureImageInfo.rcCapRect.Right = 0;
            m_CaptureImageInfo.rcCapRect.Top = 0;
            m_CaptureImageInfo.rcCapRect.Bottom = 0;
            m_CaptureImageInfo.dwDurationMode = (uint)DURATIONMODE.DURATION_COUNT;
            m_CaptureImageInfo.dwDuration = 1;
            m_CaptureImageInfo.dwVersion = 1;
            while (AVerCapAPI.AVerCaptureImageStart(m_hCaptureDevice, ref m_CaptureImageInfo) != 0)
            {
                //若截圖失敗，會等1000ms後再次截圖
                Thread.Sleep(1000);
            }
            //@ 截圖 [End]
            Statustext($"< {Criteriastr.fix_num[Criteriaint.kvm]} > {sn} Checking..."); //更新狀態列
            //@ 影像判斷 [Start]
            while (!File.Exists("Result.png")) Thread.Sleep(250); //影像存在即退出循環
            Thread.Sleep(250);
            Bitmap img = new Bitmap("Result.png"); //開啟影像讀取
            int[,] colorlist;
            string item_result, all_result = "PASS", error_msg = "";
            string[] item_string = { "Ave", "Max", "Min" };
            /* img : 影像物件
             * colorlist : 當前像素數值 {[ave R, ave G, ave B], [max R, max G, max B], [min R, min G, min B]}
             * item_result : 當前像素判定結果
             * all_result : 整個區域判定結果
             * item_string : 判定類型
             */
            Criteriastr.log = "[GET]\r\n";
            for (int i = 0; i < Criteriaint.image_count; i++)
            {
                if (img.Width - 1 < Criteriaint.range[i, 2])
                {
                    all_result = "FAIL";
                    error_msg += (error_msg == "") ? $"{Criteriastr.image_id[i]}" : $", {Criteriastr.image_id[i]}";
                    for (int j = 0; j < 3; j++)
                    {
                        Criteriastr.log += $"ImageGet_{i + 1}_{item_string[j]}=FAIL:[Out of Range, Width({img.Width - 1}/{Criteriaint.range[i, 2]})];\r\n";
                    }
                    continue;
                }
                if (img.Height - 1 < Criteriaint.range[i, 3])
                {
                    all_result = "FAIL";
                    error_msg += (error_msg == "") ? $"{Criteriastr.image_id[i]}" : $", {Criteriastr.image_id[i]}";
                    for (int j = 0; j < 3; j++)
                    {
                        Criteriastr.log += $"ImageGet_{i + 1}_{item_string[j]}=FAIL:[Out of Range, Height({img.Height - 1}/{Criteriaint.range[i, 3]})];\r\n";
                    }
                    continue;
                }
                colorlist = AverageColor(img, Criteriaint.range[i, 0], Criteriaint.range[i, 1], Criteriaint.range[i, 2], Criteriaint.range[i, 3]);
                for (int j = 0; j < 3; j++)
                {
                    item_result = "PASS";
                    for (int k = 0; k < 3; k++)
                    {
                        if (!CheckColor(colorlist[j, k], Criteriaint.interval[i, j, k * 2], Criteriaint.interval[i, j, k * 2 + 1]))
                        {
                            item_result = "FAIL";
                            all_result = "FAIL";
                            error_msg += (error_msg == "") ? $"{Criteriastr.image_id[i]}_{item_string[j]}" : $", {Criteriastr.image_id[i]}_{item_string[j]}";
                        }
                    }
                    Criteriastr.log += $"ImageGet_{i + 1}_{item_string[j]}={item_result}:[{colorlist[j, 0]},{colorlist[j, 1]},{colorlist[j, 2]}];\r\n";
                }
            }
            img.Dispose(); //關閉影像讀取
            Criteriastr.log += "\r\n[EXP]\r\n";
            for (int i = 0; i < Criteriaint.image_count; i++)
            {
                Criteriastr.log += $"ImageCheck_{i + 1}_Range=[{Criteriaint.range[i, 0]},{Criteriaint.range[i, 1]}] ~ [{Criteriaint.range[i, 2]},{Criteriaint.range[i, 3]}];\r\n";
                for (int j = 0; j < 3; j++)
                {
                    Criteriastr.log += $"ImageCheck_{i + 1}_{item_string[j]}=[{Criteriaint.interval[i, j, 0]}~{Criteriaint.interval[i, j, 1]}],[{Criteriaint.interval[i, j, 2]}~{Criteriaint.interval[i, j, 3]}],[{Criteriaint.interval[i, j, 4]}~{Criteriaint.interval[i, j, 5]}];\r\n";
                }
            }
            Criteriastr.log =
                $"SN={sn};\r\n" +
                $"RESULT={all_result};\r\n" +
                $"Error_Msg=;\r\n" +
                $"FixtureID={Criteriastr.fix_num[Criteriaint.kvm]};\r\n" +
                $"DATE={DateTime.Now:yyyyMMdd_HHmmss};\r\n\r\n" +
                $"{Criteriastr.log}";
            //@ 影像判斷 [End]
            Statustext($"< {Criteriastr.fix_num[Criteriaint.kvm]} > {sn} Result : {all_result}"); //更新狀態列
            if (File.Exists(image_address)) File.Delete(image_address);
            File.Move("Result.png", image_address);
        }

        private int[,] AverageColor(Bitmap img, int sx, int sy, int ex, int ey) //[IMG] 計算image範圍內的average, max, min
        {
            int[,] mainarray = {
                { img.GetPixel(sx,sy).R, img.GetPixel(sx,sy).G, img.GetPixel(sx,sy).B },
                { img.GetPixel(sx,sy).R, img.GetPixel(sx,sy).G, img.GetPixel(sx,sy).B },
                { img.GetPixel(sx,sy).R, img.GetPixel(sx,sy).G, img.GetPixel(sx,sy).B }
            };
            // [ave R, ave G, ave B]
            // [max R, max G, max B]
            // [min R, min G, min B]
            int nr, ng, nb, count = 0;
            for (int i = sx; i <= ex; i++)
            {
                for (int j = sy; j <= ey; j++)
                {
                    nr = img.GetPixel(i, j).R;
                    ng = img.GetPixel(i, j).G;
                    nb = img.GetPixel(i, j).B;
                    mainarray[0, 0] += nr; //ave R
                    mainarray[0, 1] += ng; //ave G
                    mainarray[0, 2] += nb; //ave B
                    mainarray[1, 0] = (nr > mainarray[1, 0]) ? nr : mainarray[1, 0]; //max R
                    mainarray[1, 1] = (ng > mainarray[1, 1]) ? ng : mainarray[1, 1]; //max G
                    mainarray[1, 2] = (nb > mainarray[1, 2]) ? nb : mainarray[1, 2]; //max B
                    mainarray[2, 0] = (nr < mainarray[2, 0]) ? nr : mainarray[2, 0]; //min R
                    mainarray[2, 1] = (ng < mainarray[2, 1]) ? ng : mainarray[2, 1]; //min G
                    mainarray[2, 2] = (nr < mainarray[2, 2]) ? nb : mainarray[2, 2]; //min B
                    count++;
                }
            }
            mainarray[0, 0] /= count;
            mainarray[0, 1] /= count;
            mainarray[0, 2] /= count;
            return mainarray;
        }

        private bool CheckColor(int get_color, int min_color, int max_color) //像素判定結果
        {
            // min_color <= get_color <= max_color : PASS，反之FAIL
            if (get_color < min_color) return false;
            if (get_color > max_color) return false;
            return true;
        }

        private void Send_PLC(string sendBuffer) //傳送字串給PLC (切換KVM)
        {
            if (sendBuffer != "")
            {
                Byte[] buffer = Encoding.ASCII.GetBytes(sendBuffer + "\r") as Byte[];
                serial_kvm.Write(buffer, 0, buffer.Length);
            }
        }

        private string Screen_resolutaion(uint i) //獲取解析度
        {
            switch (i)
            {
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_640X480:
                    return "640x480";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_704X576:
                    return "704x576";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_720X480:
                    return "720x480";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_720X576:
                    return "720x576";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1920X1080:
                    return "1920x1080";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_160X120:
                    return "160x120";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_176X144:
                    return "176x144";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_240X176:
                    return "240x176";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_240X180:
                    return "240x180";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_320X240:
                    return "320x240";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_352X240:
                    return "352x240";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_352X288:
                    return "352x288";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_640X240:
                    return "640x240";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_640X288:
                    return "640x288";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_720X240:
                    return "720x240";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_720X288:
                    return "720x288";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_80X60:
                    return "80x60";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_88X72:
                    return "88x72";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_128X96:
                    return "128x96";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_640X576:
                    return "640x576";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_180X120:
                    return "180x120";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_180X144:
                    return "180x144";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_360X240:
                    return "360x240";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_360X288:
                    return "360x288";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_768X576:
                    return "768x576";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_384x288:
                    return "384x288";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_192x144:
                    return "192x144 ";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1280X720:
                    return "1280x720";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1024X768:
                    return "1024x768";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1280X800:
                    return "1280x800";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1280X1024:
                    return "1280x1024";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1440X900:
                    return "1440x900";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1600X1200:
                    return "1600x1200";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1680X1050:
                    return "1680x1050";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_800X600:
                    return "800x600";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1280X768:
                    return "1280x768";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1360X768:
                    return "1360x768";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1152X864:
                    return "1152x864";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1280X960:
                    return "1280x960";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_702X576:
                    return "702x576";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_720X400:
                    return "720x400";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1152X900:
                    return "1152x900";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1360X1024:
                    return "1360x1024";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1366X768:
                    return "1366x768";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1400X1050:
                    return "1400x1050";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1440X480:
                    return "1440x480";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1440X576:
                    return "1440x576";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1600X900:
                    return "1600x900";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1920X1200:
                    return "1920x1200";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1440X1080:
                    return "1440x1080";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1600X1024:
                    return "1600x1024";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_3840X2160:
                    return "3840x2160";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1152X768:
                    return "1152x768";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_176X120:
                    return "176x120";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_704X480:
                    return "704x480";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1792X1344:
                    return "1792x1344";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1856X1392:
                    return "1856x1392";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_1920X1440:
                    return "1920x1440";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_2048X1152:
                    return "2048x1152";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_2560X1080:
                    return "2560x1080";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_2560X1440:
                    return "2560x1440";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_2560X1600:
                    return "2560x1600";
                case (uint)VIDEORESOLUTION.VIDEORESOLUTION_4096X2160:
                    return "4096x2160";
            }
            return "";
        }

        private bool Read_Screen() //xml讀取
        {
            string[] list_string;
            int data_int = 0;
            XmlNode node;
            string[] item_string = { "ave", "max", "min" };

            XmlDocument x = new XmlDocument();
            try
            {
                x.Load(Criteriastr.argv);
            }
            catch (Exception ex)
            {
                Statustext(ex.Message);
                return false;
            }
            //check doc & main
            if (x.SelectSingleNode("/doc") == null)
            {
                Statustext("Not found : <doc>");
                return false;
            }
            //Get 標題
            node = x.SelectSingleNode("/doc/title");
            if (node == null)
            {
                Statustext("Not found : <doc> <title>");
                return false;
            }
            Criteriastr.title = node.InnerText.Trim();
            //Get backup路徑
            node = x.SelectSingleNode("/doc/backup");
            if (node == null)
            {
                Statustext("Not found : <doc> <backup>");
                return false;
            }
            Criteriastr.backup = node.InnerText.Trim();
            //Get 溝通路經
            node = x.SelectSingleNode("/doc/shopfloor");
            if (node == null)
            {
                Statustext("Not found : <doc> <shopfloor>");
                return false;
            }
            Criteriastr.shopfloor = node.InnerText.Trim();
            //Get 鮑率
            node = x.SelectSingleNode("/doc/baud");
            if (node == null)
            {
                Statustext("Not found : <doc> <baud>");
                return false;
            }
            Criteriastr.baud = node.InnerText.Trim();
            //Get 等待時間
            node = x.SelectSingleNode("/doc/wait");
            if (node == null)
            {
                Statustext("Not found : <doc> <wait>");
                return false;
            }
            data_string = node.InnerText.Trim();
            if (!int.TryParse(data_string, out data_int))
            {
                Statustext($"\r\n<doc>\r\n  <wait>{data_string}<wait>\r\n[Not Number]");
                return false;
            }
            Criteriaint.wait = int.Parse(data_string);
            //Get status
            node = x.SelectSingleNode("/doc/status");
            if (node == null)
            {
                Statustext("Not found : <doc> <status>");
                return false;
            }
            data_string = node.InnerText.Trim();
            if (!int.TryParse(data_string, out data_int))
            {
                Statustext($"\r\n<doc>\r\n <status>{node.InnerText}</status>\r\n[Not Number]");
                return false;
            }
            Criteriaint.status_max = int.Parse(data_string);
            if (Criteriaint.status_max < 500)
            {
                Criteriaint.status_max = 500;
                Statustext($"Message word limit can't less 500, send 500 now.");
            }

            XmlNodeList nodelist;
            if (x.SelectSingleNode("/doc/fixtures") == null)
            {
                Statustext("Not found : <doc> <fixtures>");
                return false;
            }
            //Get 治具清單
            nodelist = x.SelectNodes("/doc/fixtures/fixture/@id");
            Criteriaint.fix_count = nodelist.Count;
            if (Criteriaint.fix_count == 0)
            {
                Statustext("Not found : <doc> <fixtures> <fixture id=\"\">");
                return false;
            }
            Criteriastr.fix_id = new string[Criteriaint.fix_count];
            Criteriastr.fix_send = new string[Criteriaint.fix_count];
            Criteriastr.fix_num = new string[Criteriaint.fix_count];
            for (int i = 0; i < Criteriaint.fix_count; i++)
            {
                Criteriastr.fix_id[i] = nodelist[i].InnerText;
                //Get 治具 PLC
                node = x.SelectSingleNode($"/doc/fixtures/fixture[{i + 1}]/plc");
                if (node == null)
                {
                    Statustext($"Not found : <doc> <fixtures> <fixture id=\"{Criteriastr.fix_id[i]}\"> <plc>");
                    return false;
                }
                Criteriastr.fix_send[i] = node.InnerText.Trim();
                //Get 治具 編號
                node = x.SelectSingleNode($"/doc/fixtures/fixture[{i + 1}]/num");
                if (node == null)
                {
                    Statustext($"Not found : <doc> <fixtures> <fixture id=\"{Criteriastr.fix_id[i]}\"> <num>");
                    return false;
                }
                Criteriastr.fix_num[i] = node.InnerText.Trim();
            }

            if (x.SelectSingleNode("/doc/images") == null)
            {
                Statustext("Not found : <doc> <images>");
                return false;
            }
            //Get Image 清單
            nodelist = x.SelectNodes("/doc/images/image/@id");
            Criteriaint.image_count = nodelist.Count;
            if (Criteriaint.image_count == 0)
            {
                Statustext("Not found : <doc> <fixtures> <image id=\"\">");
                return false;
            }
            Criteriaint.range = new int[Criteriaint.image_count, 4];
            Criteriaint.interval = new int[Criteriaint.image_count, 3, 6];
            Criteriastr.image_id = new string[Criteriaint.image_count];
            for (int i = 0; i < Criteriaint.image_count; i++)
            {
                Criteriastr.image_id[i] = nodelist[i].InnerText;
                //Get Image Range
                node = x.SelectSingleNode($"/doc/images/image[{i + 1}]/range");
                if (node == null)
                {
                    Statustext($"Not found : <doc> <images> <image id=\"{Criteriastr.image_id[i]}\"> <range>");
                    return false;
                }
                list_string = node.InnerText.Split(',');
                if (list_string.Length != 4)
                {
                    Statustext($"\r\n<doc>\r\n <images>\r\n  <image id=\"{Criteriastr.image_id[i]}\">\r\n   <range>{node.InnerText}<range>\r\n[Number count not 4]");
                    return false;
                }
                for (int j = 0; j < 4; j++)
                {
                    data_int = 0;
                    data_string = list_string[j].Trim();
                    if (!int.TryParse(data_string, out data_int))
                    {
                        Statustext($"\r\n<doc>\r\n <images>\r\n  <image id=\"{Criteriastr.image_id[i]}\">\r\n   <range>{node.InnerText}<range>\r\n[Not Number]");
                        return false;
                    }
                    Criteriaint.range[i, j] = int.Parse(data_string);
                }
                //Get Image Ave[,0,], Max[,1,], Min[,2,]
                for (int j = 0; j < 3; j++)
                {
                    node = x.SelectSingleNode($"/doc/images/image[{i + 1}]/{item_string[j]}");
                    if (node == null)
                    {
                        Statustext($"Not found : <doc> <images> <image id=\"{Criteriastr.image_id[i]}\"> <{item_string[j]}>");
                        return false;
                    }
                    list_string = node.InnerText.Split(',');
                    if (list_string.Length != 6)
                    {
                        Statustext($"\r\n<doc>\r\n <images>\r\n  <image id=\"{Criteriastr.image_id[i]}\">\r\n   <{item_string[j]}>{node.InnerText}<{item_string[j]}>\r\n[Number count not 6]");
                        return false;
                    }
                    for (int k = 0; k < 6; k++)
                    {
                        data_int = 0;
                        data_string = list_string[k].Trim();
                        if (!int.TryParse(data_string, out data_int))
                        {
                            Statustext($"\r\n<doc>\r\n <images>\r\n  <image id=\"{Criteriastr.image_id[i]}\">\r\n   <{item_string[j]}>{node.InnerText}<{item_string[j]}>\r\n[Not Number]");
                            return false;
                        }
                        Criteriaint.interval[i, j, k] = int.Parse(data_string);
                    }
                }
            }
            
            /*/Get kvmoff
            node = x.SelectSingleNode("/doc/kvmoff");
            if (node == null)
            {
                Statustext("Not found : <doc> <kvmoff>");
                return false;
            }
            //Get kvmon
            node = x.SelectSingleNode("/doc/kvmon");
            if (node == null)
            {
                Statustext("Not found : <doc> <kvmon>");
                return false;
            }*/
            
            //Get 解析度
            node = x.SelectSingleNode("/doc/resolutaion");
            if (node != null)
            {
                Criteriastr.resolutaion = node.InnerText.Trim();
            }
            //全螢幕 Hide
            if (File.Exists("hide.cfg"))
            {
               // Statustext($"<hide.cfg> Full screen hide bar!");
                bool_hide = true;
            }
            //全螢幕 full
            if (File.Exists("full.cfg"))
            {
                //Statustext($"<full.cfg> Full screen real!");
                bool_full = true;
            }
            return true;
        }

        private void label_vga_Click(object sender, EventArgs e)
        {

        }

        int second = 30;
        
        
        private void timer_Tick(object sender, EventArgs e)
        {

            second -= 1;
            textBox_timer.Text = "Wait " + second.ToString();

            /*if (Criteriabool.testdetected) //偵測有沒有測試
            {
                second += 15;
                Criteriabool.testdetected = false;
            } */  



            if (second <= 0)
            {
                timer.Stop();
                second = 30;
                textBox_timer.Visible = false;
                button_vga.Enabled = true;
                if (Criteriabool.vgarun == true)
                {                                        
                    button_vga.PerformClick();
                }

            }

        }


        private void textBox_TextChanged(object sender, EventArgs e)
        {

        }



        private void pictureBoxShowMain_Click(object sender, EventArgs e)
        {

        }

        private void textBox_status_TextChanged(object sender, EventArgs e)
        {

        }

        private void group_manual_Enter(object sender, EventArgs e)
        {

        }

        private void group_manual_Enter_1(object sender, EventArgs e)
        {

        }

        private void group_vga_Enter(object sender, EventArgs e)
        {

        }

        private void group_kvm_Enter(object sender, EventArgs e)
        {

        }

        private void label_kvmnum_Click(object sender, EventArgs e)
        {

        }

        private void group_xml_Enter(object sender, EventArgs e)
        {

        }

        private void label_clock_Click(object sender, EventArgs e)
        {

        }

        private void label_checking_Click(object sender, EventArgs e)
        {

        }

        private void group_auto_Enter(object sender, EventArgs e)
        {

        }

        private void Statustext(string s)
        {
            if (textBox_status.InvokeRequired)
            {
                textBox_status.Invoke(new delegateStatustext(Statustext), new object[] { s });
            }
            else
            {
                status_string = $"[ {DateTime.Now:yyyy/MM/dd HH:mm:ss} ]　{s}\r\n";
                if (!Directory.Exists("Log")) Directory.CreateDirectory("Log");
                File.AppendAllText($"Log\\StatusText_{DateTime.Now:yyyyMMdd}.log", status_string);
                data_int = textBox_status.Text.Length;
                if (data_int > Criteriaint.status_max)
                {
                    textBox_status.Text = textBox_status.Text.Remove(0, Criteriaint.status_max / 2);
                }
                textBox_status.AppendText(status_string);
            }
        }

        private void pictureBoxShowMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!Criteriabool.vgarun)
            {
                Statustext("Can't use full screen!\r\n\tPlease connect VGA!");
                return;
            }
            /*ScreenChange();*/
        }
        private void ScreenChange()
        {
            pictureBoxShowMain.Dock = (pictureBoxShowMain.Dock == DockStyle.Fill) ? DockStyle.None : DockStyle.Fill;
            group_xml.Visible = pictureBoxShowMain.Dock != DockStyle.Fill;
            group_kvm.Visible = pictureBoxShowMain.Dock != DockStyle.Fill;
            group_vga.Visible = pictureBoxShowMain.Dock != DockStyle.Fill;
            group_auto.Visible = pictureBoxShowMain.Dock != DockStyle.Fill;
            textBox_status.Visible = pictureBoxShowMain.Dock != DockStyle.Fill;
            label_kvmnum.Visible = pictureBoxShowMain.Dock != DockStyle.Fill;
            if (bool_full && pictureBoxShowMain.Dock == DockStyle.Fill) this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = pictureBoxShowMain.Dock == DockStyle.Fill;
            this.WindowState = pictureBoxShowMain.Dock == DockStyle.Fill ? FormWindowState.Maximized : FormWindowState.Normal;
            combo_kvm.Location = pictureBoxShowMain.Dock == DockStyle.Fill ? new Point(this.Width - 202, this.Height / 2) : point_kvm;
            label_checking.Location = pictureBoxShowMain.Dock == DockStyle.Fill ? new Point(this.Width - 170, this.Height / 2) : point_checking;
            label_clock.Location = pictureBoxShowMain.Dock == DockStyle.Fill ? new Point(this.Width - 132, this.Height / 2 - 40) : point_clock;
            if (bool_hide)
            {
                combo_kvm.Visible = pictureBoxShowMain.Dock != DockStyle.Fill;
                label_clock.Visible = pictureBoxShowMain.Dock != DockStyle.Fill;
            }
            else
            {
                label_checking.Visible = pictureBoxShowMain.Dock == DockStyle.Fill && bool_checking;
            }
            if (bool_full && pictureBoxShowMain.Dock != DockStyle.Fill) this.FormBorderStyle = FormBorderStyle.FixedSingle;
            SetVideoWindow();
            combo_kvm.Focus();
        }
        private void FolderReConnect(string s)
        {
            while (!bw_auto.CancellationPending)
            {
                if (Directory.Exists(s))
                {
                    Statustext($"Retry Connect : Successful\r\n\t{s}");
                    AutoTestError(false);
                    Thread.Sleep(500);
                    return;
                }
                else
                {
                    Statustext($"Retry Connect : Failed\r\n\t{s}");
                    Thread.Sleep(2000);
                }
            }
        }

        private void KVMReConnect()
        {
            while (!bw_auto.CancellationPending)
            {
                try
                {
                    serial_kvm.Open();
                    Statustext($"Retry {serial_kvm.PortName} : Successful");
                    Criteriabool.kvmerror = false;
                    AutoTestError(false);
                    Thread.Sleep(1000);
                    return;
                }
                catch (Exception)
                {
                    Statustext($"Retry {serial_kvm.PortName} : Failed");
                    Thread.Sleep(2000);
                }
            }
        }
        private void ChangeLabelKvm(string s)
        {
            if (label_kvmnum.InvokeRequired)
            {
                label_kvmnum.Invoke(new delegateLabelKvm(ChangeLabelKvm), new object[] { s });
            }
            else
            {
                label_kvmnum.Text = s;
            }
        }
        private void ClockRunning()
        {
            if (label_kvmnum.InvokeRequired)
            {
                label_kvmnum.Invoke(new delegateLabelClock(ClockRunning), new object[] {});
            }
            else
            {
                label_clock.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }
        
        private void AutoTestError(bool b)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new delegateErrorCheck(AutoTestError), new object[] { b });
            }
            else
            {
                if (b)
                {
                    //Error ON
                    this.Text = $"{Criteriastr.title} : Automation error...";
                    this.BackColor = Color.Red;
                    label_clock.ForeColor = Color.Red;
                }
                else
                {
                    //Error OFF
                    this.Text = $"{Criteriastr.title} : Automation running...";
                    this.BackColor = SystemColors.Control;
                    label_clock.ForeColor = SystemColors.Control;
                }
            }
        }
        private void TestingStart(bool b)
        {
            if (this.InvokeRequired)
            {
                label_kvmnum.Invoke(new delegateTestingStart(TestingStart), new object[] { b });
            }
            else
            {
                if (b)
                {
                    //Testing Start
                    button_auto.Enabled = false;
                    if(!bool_hide) label_checking.Visible = pictureBoxShowMain.Dock == DockStyle.Fill;
                    combo_kvm.Visible = false;
                }
                else
                {
                    //Testing Over
                    button_auto.Enabled = true;
                    label_checking.Visible = false;
                    combo_kvm.Visible = true;
                }
            }
        }

        public void SelectKVM(int num)
        {
            if (combo_kvm.InvokeRequired)
            {
                combo_kvm.Invoke(new delegateKvmChange(SelectKVM), new object[] { num });
            }
            else
            {
                combo_kvm.SelectedIndex = num;
            }
        }
    }
}
