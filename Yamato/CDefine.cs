using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yamato
{
    //--------------------------------------------------------------
    #region 列挙型
    /// <summary>
    /// ヤマト配送伝票番号要素
    /// </summary>
    public enum EnumYamatoDeliNoItem
    {
        /// <summary>お客様側管理番号</summary>
        USER_ID = 0,
        /// <summary>送り状種類</summary>
        INVOICE_CLASS = 1,
        /// <summary>伝票番号</summary>
        DELIVERY_NO = 3,
        /// <summary>お届け先名</summary>
        NAME = 15,

        MAX
    }
    /// <summary>
    /// カラーミー配送伝票番号要素
    /// </summary>
    public enum EnumColorMeDeliNoItem
    {
        /// <summary>注文ID</summary>
        ORDER_ID = 0,
        /// <summary>配送先ID</summary>
        SHIPPING_ID,
        /// <summary>配送伝票番号</summary>
        DELIVERY_NO,

        MAX
    }
    /// <summary>
    /// アマゾン配送伝票番号要素
    /// </summary>
    public enum EnumAmazonDeliNoItem
    {
        /// <summary>注文ID</summary>
        ORDER_ID = 0,
        /// <summary>出荷日</summary>
        SHIPPING_DATE = 3,
        /// <summary>配送業者コード</summary>
        CARROER_CODE = 4,
        /// <summary>配送業者名</summary>
        CARROER_NAME = 5,
        /// <summary>トラッキング番号</summary>
        TRACKING_NUMVER = 6,
        /// <summary>代金引換</summary>
        COD_COLLECTION_METHOD = 8,

        MAX
    }
    /// <summary>
    /// EC-CUBE配送伝票番号要素
    /// </summary>
    public enum EnumEcCubeDeliNoItem
    {
        /// <summary>注文ID</summary>
        ORDER_ID = 0,
        /// <summary>お名前</summary>
        NAME,
        /// <summary>配送伝票番号</summary>
        DELIVERY_NO,

        MAX
    }
    #endregion
    //--------------------------------------------------------------
    #region 構造体
    /// <summary>
    /// ヤマト配送伝票番号データクラス
    /// </summary>
    public class CYamatoDeliNoData
    {
        //--------------------------------------------------------------
        /// <summary>お客様側管理番号</summary>
        public string strUserId;
        /// <summary>送り状種類</summary>
        public int iInvoiceClass;
        /// <summary>伝票番号</summary>
        public string strDeliveryNo;
        /// <summary>お届け先名</summary>
        public string strName;
        //--------------------------------------------------------------
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CYamatoDeliNoData()
        {
            Init();
        }
        /// <summary>
        /// 初期化関数
        /// </summary>
        public void Init()
        {
            strUserId = "";
            iInvoiceClass = 0;
            strDeliveryNo = "";
            strName = "";
        }
    }
    #endregion
    //--------------------------------------------------------------
    /// <summary>
    /// 定数宣言クラス
    /// </summary>
    class CDefine
    {
    }
}
