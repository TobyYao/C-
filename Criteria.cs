namespace Criteria
{
    public class Criteriastr
    {
        
        public static string argv = "Screen.xml"; //標題
        public static string ver = "v2.02"; //版本
        public static string title; //標題
        public static string backup; //截圖、log備份路徑
        public static string shopfloor; //與MB溝通路徑
        public static string baud; //預設鮑率
        public static string resolutaion = string.Empty; //解析度
        public static string log; //tpm內容
        public static string[] fix_send; //給PLC字串 (切換KVM)
        public static string[] fix_id; //xml mac id
        public static string[] fix_num; //mac編號
        public static string[] image_id; //xml image id
        public static string kvmoff = "@00FA00000000001023100640000010174*";
        public static string kvmon = "@00FA00000000001023100640000010075*";

    }
    public class Criteriaint
    {
        public static int kvm; //當前kvm編號 (0開始)
        public static int[,] range; //影像檢查像素範圍
        public static int[,,] interval; //影像檢查顏色判定區間
        public static int fix_count; //mac 數量
        public static int image_count; //image check 數量
        public static int wait; //切換kvm後，等待數秒後截圖
        public static int status_max = 50000; //狀態列字數上限
        public static uint[] resolutaion_list; //解析度列表
        public static uint resolutaion_num; //解析度編號

    }
    public class Criteriabool
    {
        public static bool testdetected = false;
        public static bool vgarun = false; //KVM連線開關
        public static bool autorun = false; //自動化開關
        public static bool autoerror = false; //自動化異常開關
        public static bool kvmerror = false; //KVM異常開關
    }
}
