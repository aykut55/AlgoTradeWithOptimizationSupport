namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Konfirmasyon tetikleyici modu
    /// </summary>
    public enum ConfirmationTrigger
    {
        /// <summary>
        /// Sadece kar esiginde konfirme et (trend onay)
        /// Sanal pozisyon kar yaziyorsa, trend dogru yonde demektir
        /// </summary>
        KarOnly,

        /// <summary>
        /// Sadece zarar esiginde konfirme et (reversal beklentisi)
        /// Sanal pozisyon zarar yaziyorsa, fiyat donecek demektir
        /// </summary>
        ZararOnly,

        /// <summary>
        /// Her ikisinde de konfirme et
        /// Kar veya zarar esigine ulasinca gercek sinyal uret
        /// </summary>
        Both
    }
}
