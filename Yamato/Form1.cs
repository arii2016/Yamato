using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Yamato
{
    public partial class Form1 : Form
    {
        //--------------------------------------------------------------
        // 読み込みデーター
        List<string[]> m_listReadData = new List<string[]>();
        List<CYamatoDeliNoData> m_listYamatoDeliNo = new List<CYamatoDeliNoData>();
        //--------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
        }
        //--------------------------------------------------------------
        private void Pl_FileLoad_DragEnter(object sender, DragEventArgs e)
        {
            //コントロール内にドラッグされたとき実行される
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
            }
        }
        //--------------------------------------------------------------
        private void Pl_FileLoad_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたすべてのファイル名を取得する
            string[] FileNameList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            for (int i = 0; i < FileNameList.Length; i++)
            {
                if (LoadDataCsv(FileNameList[i]) == false)
                {
                    MessageBox.Show("読み込みエラー");
                    return;
                }
                // コンバート処理
                if (ConvertCsv(FileNameList[i]) == false)
                {
                    MessageBox.Show("書き込みエラー");
                    return;
                }
            }
        }
        //--------------------------------------------------------------
        // ファイルからデータ読み込み
        public bool LoadDataCsv(string strFileName)
        {
            // ヘッダー
            string[] strHeader;

            // ファイルを読み込む
            StreamReader clsSr;
            try
            {
                clsSr = new StreamReader(strFileName, System.Text.Encoding.GetEncoding("shift_jis"));
                try
                {
                    string strLine;

                    // 1行目読み込み
                    if ((strLine = clsSr.ReadLine()) == null)
                    {
                        throw new FormatException();
                    }
                    // ヘッダーの文字列を読み、種類を判断する
                    strHeader = strLine.Split(',');
                    if (strHeader[0] != "\"お客様管理番号\"")
                    {
                        throw new FormatException();
                    }
                    // データーを読み込む                
                    while ((strLine = clsSr.ReadLine()) != null)
                    {
                        if (strLine == "")
                        {
                            break;
                        }
                        // 全てのデーターを格納
                        m_listReadData.Add(strLine.Split(','));
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    clsSr.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        //--------------------------------------------------------------
        // コンバート処理実行
        public bool ConvertCsv(string strFileName)
        {
            // 配送伝票番号情報出力ファイル名
            string strColorMeFileName = Path.GetDirectoryName(strFileName) + "\\" + "カラーミー.csv";
            string strAmazonFileName = Path.GetDirectoryName(strFileName) + "\\" + "Amazon.txt";
            string strEcCubeFileName = Path.GetDirectoryName(strFileName) + "\\" + "EC-CUBE.csv";

            // 両サイドの"を取り外す
            for (int i = 0; i < m_listReadData.Count; i++)
            {
                for (int j = 0; j < m_listReadData[0].Length; j++)
                {
                    m_listReadData[i][j] = m_listReadData[i][j].Substring(1, m_listReadData[i][j].Length - 2);
                }
            }
            
            // 重複USER_IDを最新のにまとめる
            m_listYamatoDeliNo.Clear();
            for (int i = 0; i < m_listReadData.Count; i++)
            {
                if (m_listReadData[i][(int)EnumYamatoDeliNoItem.USER_ID] != "")
                {
                    bool bMacheFlg = false;
                    for (int j = 0; j < m_listYamatoDeliNo.Count; j++)
                    {
                        if (m_listYamatoDeliNo[j].strUserId != m_listReadData[i][(int)EnumYamatoDeliNoItem.USER_ID])
                        {
                            continue;
                        }
                        bMacheFlg = true;
                        if (long.Parse(m_listYamatoDeliNo[j].strDeliveryNo) < long.Parse(m_listReadData[i][(int)EnumYamatoDeliNoItem.DELIVERY_NO]))
                        {
                            m_listYamatoDeliNo[j].strDeliveryNo = m_listReadData[i][(int)EnumYamatoDeliNoItem.DELIVERY_NO];
                        }
                        break;
                    }
                    if (bMacheFlg == false)
                    {
                        CYamatoDeliNoData clsData = new CYamatoDeliNoData();

                        clsData.strUserId = m_listReadData[i][(int)EnumYamatoDeliNoItem.USER_ID];
                        clsData.iInvoiceClass = int.Parse(m_listReadData[i][(int)EnumYamatoDeliNoItem.INVOICE_CLASS]);
                        clsData.strDeliveryNo = m_listReadData[i][(int)EnumYamatoDeliNoItem.DELIVERY_NO];
                        clsData.strName = m_listReadData[i][(int)EnumYamatoDeliNoItem.NAME];

                        m_listYamatoDeliNo.Add(clsData);
                    }
                }
            }

            // 書き込み
            for (int i = 0; i < m_listYamatoDeliNo.Count; i++)
            {
                if (m_listYamatoDeliNo[i].strUserId.Substring(0, 1) == "1")
                {
                    // カラーミー
                    string[] strColorMeDeliNo = new string[(int)EnumColorMeDeliNoItem.MAX];

                    // 売上ID
                    strColorMeDeliNo[(int)EnumColorMeDeliNoItem.ORDER_ID] = m_listYamatoDeliNo[i].strUserId.Substring(1, m_listYamatoDeliNo[i].strUserId.Length - 1);
                    // 配送先ID
                    strColorMeDeliNo[(int)EnumColorMeDeliNoItem.SHIPPING_ID] = "";
                    // 配送伝票番号
                    strColorMeDeliNo[(int)EnumColorMeDeliNoItem.DELIVERY_NO] = m_listYamatoDeliNo[i].strDeliveryNo;

                    // 書き込む
                    StreamWriter clsSw;
                    try
                    {
                        clsSw = new StreamWriter(strColorMeFileName, true, Encoding.GetEncoding("Shift_JIS"));
                    }
                    catch
                    {
                        return false;
                    }
                    for (int j = 0; j < (int)EnumColorMeDeliNoItem.MAX; j++)
                    {
                        if (j != 0)
                        {
                            clsSw.Write(",");
                        }
                        clsSw.Write("{0}", strColorMeDeliNo[j]);
                    }
                    clsSw.Write("\n");

                    clsSw.Flush();
                    clsSw.Close();

                }
                else if (m_listYamatoDeliNo[i].strUserId.Substring(0, 1) == "2")
                {
                    // Amazon
                    string[] strAmazonDeliNo = new string[(int)EnumAmazonDeliNoItem.MAX];
                    bool bHeaderFlg = false;

                    // 売上ID
                    strAmazonDeliNo[(int)EnumAmazonDeliNoItem.ORDER_ID] = m_listYamatoDeliNo[i].strUserId.Substring(1, m_listYamatoDeliNo[i].strUserId.Length - 1);
                    // 出荷日
                    strAmazonDeliNo[(int)EnumAmazonDeliNoItem.SHIPPING_DATE] = DateTime.Now.ToString("yyyy-MM-dd");
                    // 配送業者コード
                    strAmazonDeliNo[(int)EnumAmazonDeliNoItem.CARROER_CODE] = "Other";
                    // 配送業者名
                    strAmazonDeliNo[(int)EnumAmazonDeliNoItem.CARROER_NAME] = "ヤマト運輸";
                    // トラッキング番号
                    strAmazonDeliNo[(int)EnumAmazonDeliNoItem.TRACKING_NUMVER] = m_listYamatoDeliNo[i].strDeliveryNo;
                    // 代金引換
                    if (m_listYamatoDeliNo[i].iInvoiceClass == 2)
                    {
                        strAmazonDeliNo[(int)EnumAmazonDeliNoItem.COD_COLLECTION_METHOD] = "DirectPayment";
                    }
                    else
                    {
                        strAmazonDeliNo[(int)EnumAmazonDeliNoItem.COD_COLLECTION_METHOD] = "";
                    }

                    // ファイルが存在しない場合には、ヘッダーを追加
                    if (System.IO.File.Exists(strAmazonFileName) == false)
                    {
                        bHeaderFlg = true;
                    }

                    // 書き込む
                    StreamWriter clsSw;
                    try
                    {
                        clsSw = new StreamWriter(strAmazonFileName, true, Encoding.GetEncoding("Shift_JIS"));
                    }
                    catch
                    {
                        return false;
                    }
                    // データ書き込み
                    if (bHeaderFlg)
                    {
                        clsSw.Write("TemplateType=OrderFulfillment	Version=2011.1102	この行はAmazonが使用しますので変更や削除しないでください。						\n");
                        clsSw.Write("注文番号	注文商品番号	出荷数	出荷日	配送業者コード	配送業者名	お問い合わせ伝票番号	配送方法	代金引換\n");
                        clsSw.Write("order-id	order-item-id	quantity	ship-date	carrier-code	carrier-name	tracking-number	ship-method	cod-collection-method\n");
                    }
                    for (int j = 0; j < (int)EnumAmazonDeliNoItem.MAX; j++)
                    {
                        if (j != 0)
                        {
                            clsSw.Write("	");
                        }
                        clsSw.Write("{0}", strAmazonDeliNo[j]);
                    }
                    clsSw.Write("\n");

                    clsSw.Flush();
                    clsSw.Close();
                }
                else if (m_listYamatoDeliNo[i].strUserId.Substring(0, 1) == "3")
                {
                    // EC-CUBE
                    string[] strEcCubeDeliNo = new string[(int)EnumEcCubeDeliNoItem.MAX];

                    // 売上ID
                    strEcCubeDeliNo[(int)EnumEcCubeDeliNoItem.ORDER_ID] = m_listYamatoDeliNo[i].strUserId.Substring(1, m_listYamatoDeliNo[i].strUserId.Length - 1);
                    // 名前
                    strEcCubeDeliNo[(int)EnumEcCubeDeliNoItem.NAME] = m_listYamatoDeliNo[i].strName;
                    // トラッキング番号
                    strEcCubeDeliNo[(int)EnumEcCubeDeliNoItem.DELIVERY_NO] = m_listYamatoDeliNo[i].strDeliveryNo;

                    // 書き込む
                    StreamWriter clsSw;
                    try
                    {
                        clsSw = new StreamWriter(strEcCubeFileName, true, Encoding.GetEncoding("Shift_JIS"));
                    }
                    catch
                    {
                        return false;
                    }
                    for (int j = 0; j < (int)EnumEcCubeDeliNoItem.MAX; j++)
                    {
                        if (j != 0)
                        {
                            clsSw.Write(",");
                        }
                        clsSw.Write("{0}", strEcCubeDeliNo[j]);
                    }
                    clsSw.Write("\n");

                    clsSw.Flush();
                    clsSw.Close();
                }
            }

            return true;
        }
        //--------------------------------------------------------------
    }
}
