public enum EventType
{
    ShowRegisterPanel,
    ShowLoginPanel,
    Hint,
    ChooseHeadIcon,
    BackPersonalInterface,

    /// <summary>
    /// 开始游戏
    /// </summary>
    RefreshUI,
    StartGame,
    //发牌
    RefreshCard,
    //下一个竞价
    Auction,
    //更新竞价结果
    RefreshBiddingResult,
    SetHarbourMaster,
    //更新金钱数
    RefreshMoney,
    //货物上下船
    OnBoard,
    OffBoard,
    //更新船位置的显示
    MoveShip,

    /// <summary>
    /// 检测位点的点击
    /// </summary>
    //开放检测
    OpenDetection,
    CloseDetection,
    RefreshWorker,

    //指示当前操作玩家
    CurrentPlayer,
    CastDice,

    //股票功能，可见和不可见
    ShareButtonAvailable,
    ShareButtonUnAvaliable,

    //货物涨价，刷新价格板
    RefreshPriceBoard,

    /// <summary>
    /// 特殊功能同伙
    /// </summary>
    //领航员
    SmallPilotOperation,
    LargePilotOperation,
    //海盗
    PirateOnBoardOpen,
    PirateOnBoardClose,
    PirateChoose,
    InsurancePart,

    //结算
    SettlementContext,
    SettlementShow,
    GameOverSettlement,
    //点击重置
    ClickReset,
    NewMoveRound,
}
