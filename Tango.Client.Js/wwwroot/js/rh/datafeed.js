var DataFeed = (function () {
    function DataFeed() {
    }
    return DataFeed;
})();
var TimeFrame;
(function (TimeFrame) {
    TimeFrame[TimeFrame["MN"] = 0] = "MN";
    TimeFrame[TimeFrame["W"] = 1] = "W";
    TimeFrame[TimeFrame["D"] = 2] = "D";
})(TimeFrame || (TimeFrame = {}));
var Symbol = (function () {
    function Symbol(name, period, onePoint) {
        this.date = [];
        this.open = [];
        this.high = [];
        this.low = [];
        this.close = [];
        this.volume = [];
        this.name = name;
        this.period = period;
        this.onePoint = onePoint;
    }
    Symbol.prototype.count = function () {
        return this.close.length;
    };
    return Symbol;
})();
