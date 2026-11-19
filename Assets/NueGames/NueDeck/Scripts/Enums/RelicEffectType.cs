namespace NueGames.NueDeck.Scripts.Enums
{
    public enum RelicEffectType
    {
        IncreaseDamage,
        IncreaseMaxHealth,
        GainMana,

        //bổ sung
        IncreaseAttackDamage,    // Tăng sát thương của các lá bài tấn công.
        // IncreaseDamageToBoss,    // Tăng sát thương khi chiến đấu với Boss(delay).
        FirstAttackBonus,        // Đòn tấn công đầu tiên mỗi trận gây thêm sát thương = value.

        Heal,       // Hồi một lượng máu.
        GainBlock,               // Nhận một lượng Block (giáp) = value.
        IncreaseBlock,           // Tăng lượng Block nhận được từ mọi nguồn, % tăng thêm = value.
        Thorns,                  // Phản lại sát thương khi bị kẻ địch tấn công, % phản sát thương = value.

        IncreaseMaxMana,         // Tăng giới hạn Mana tối đa = value.
        ReduceCardCost,          // Giảm cost của lá bài tiếp theo = value, hết hiệu lực sau khi sử dụng.
        ReduceAllCardCost,       // Giảm cost của tất cả bài, value = số cost giảm.

        DrawCard,                // Rút bài, số bài = value.
        IncreaseHandSize,        // Tăng giới hạn số lá bài trên tay = value.

        ApplyPoison,             // Gây Poison lên kẻ địch với số damage = value.
        ApplyWeak,               // Gây Weak lên kẻ địch với số turn = value.
        ApplyVulnerable,         // Gây Vulnerable lên kẻ địch với số turn = value.
        IncreaseDebuffDuration,  // Tăng thời gian tồn tại của hiệu ứng bất lợi với số turn = value.

        GainArtifact,       // Nhận Artifact ngẫu nhiên số lượng = value;
        GainStrength,            // Nhận Strength trong trận với số turn = value.
        PreventDamage,      // Chặn hoàn toàn sát thương nhận vào trong 1 hiệp, số lần chặn = value.

        gainGold,            // Nhận thêm vàng , sô lượng = value.
        ShopDiscount,    // Giảm giá vật phẩm trong Shop, % giảm = value.
        // IncreaseRareCardChance,  // Tăng tỷ lệ xuất hiện bài hiếm (delay).

        CopyLastCard,            // kích hoạt lại hiệu ứng lá bài kế tiếp sử dụng số lần kích hoạt lại = value.
        TriggerEffectTwice       // Kích hoạt lại hiệu ứng của Relic loại "triger on enquipted" hiện có trong persistant số lần kích hoạt lại = value.
    }
}