/// <reference path="pixi.js.d.ts"/>
var DataSeriesDrawingModel = (function () {
    function DataSeriesDrawingModel() {
    }
    DataSeriesDrawingModel.prototype.valueToY = function (value) {
        var chartHeight = this.frame.height - this.frame.padding * 2;
        if (this.highValue - this.lowValue != 0)
            return this.frame.top + this.frame.padding + Math.round((this.highValue - value) * chartHeight / (this.highValue - this.lowValue));
        else
            return this.frame.top + this.frame.padding;
    };
    return DataSeriesDrawingModel;
})();
var ChartRenderer = (function () {
    function ChartRenderer() {
    }
    ChartRenderer.prototype.render = function (chart) {
        var model = this.createModel(chart);
        var renderer = PIXI.autoDetectRenderer(chart.width, chart.height, { backgroundColor: 0xffffff, antialias: false });
        var ticker = PIXI.ticker.shared;
        ticker.autoStart = false;
        ticker.stop();
        //renderer.view.style.border = "1px solid #000";
        document.body.appendChild(renderer.view);
        var stage = new PIXI.Container();
        var graphics = new PIXI.Graphics();
        stage.addChild(graphics);
        this.drawModel(model, stage, graphics);
        renderer.render(stage);
    };
    ChartRenderer.prototype.createModel = function (chart) {
        var model = {
            width: chart.width ? chart.width : 800,
            height: chart.height ? chart.height : 600,
            enableGrid: chart.enableGrid,
            itemWidth: 5,
            itemsInterval: 3,
            frames: []
        };
        var frameModel = {
            model: model,
            width: model.width - 40,
            height: model.height - 20,
            left: 3,
            top: 2,
            padding: 10,
            dataSeries: [],
        };
        model.itemsOnChart = Math.round((model.width - 40 - frameModel.left) / (model.itemWidth + model.itemsInterval));
        model.currentItem = chart.dataSeries[0].close.length;
        for (var j = 0, len2 = chart.dataSeries.length; j < len2; j++) {
            frameModel.dataSeries.push(this.createDataSeriesDrawingModel(frameModel, chart.dataSeries[j]));
        }
        model.frames.push(frameModel);
        for (var i = 0, len = chart.frames ? chart.frames.length : 0; i < len; i++) {
            var frame = chart.frames[i];
            var frameModel = {
                model: model,
                width: model.width - 40,
                height: model.height - 20,
                left: 3,
                top: 2,
                padding: 10,
                dataSeries: []
            };
            for (var j = 0, len2 = frame.dataSeries.length; j < len2; j++) {
                frameModel.dataSeries.push(this.createDataSeriesDrawingModel(frameModel, frame.dataSeries[j]));
            }
            model.frames.push(frameModel);
        }
        return model;
    };
    ChartRenderer.prototype.createDataSeriesDrawingModel = function (frame, data) {
        var res = new DataSeriesDrawingModel();
        res.frame = frame;
        res.dataSeries = data;
        var begin = frame.model.currentItem - frame.model.itemsOnChart;
        if (begin < 0)
            begin = 1;
        for (var j = begin; j < frame.model.currentItem; j++) {
            if (!res.highValue || data.high[j] > res.highValue)
                res.highValue = data.high[j];
            if (!res.lowValue || data.low[j] < res.lowValue)
                res.lowValue = data.low[j];
        }
        return res;
    };
    ChartRenderer.prototype.drawModel = function (model, stage, graphics) {
        graphics.lineStyle(1, 0x000000, 1);
        for (var i = 0, len = model.frames.length; i < len; i++) {
            var frame = model.frames[i];
            graphics.drawRect(frame.left, frame.top, frame.width, frame.height);
            for (var j = 0, len2 = frame.dataSeries.length; j < len2; j++) {
                this.drawCandleStickChart(frame.dataSeries[j], stage, graphics);
            }
        }
        //var txt = new PIXI.Text('1.2001', { font: '12px Consolas', fill: 0x000000, align: 'left' });		
        //stage.addChild(txt);
    };
    ChartRenderer.prototype.drawCandleStickChart = function (dataModel, stage, graphics) {
        var model = dataModel.frame.model;
        var last, first;
        var yo, yh, yl, yc, k = 1, x, lx, rx;
        if (model.currentItem - 1 >= dataModel.dataSeries.length)
            last = dataModel.dataSeries.length - 1;
        else
            last = model.currentItem - 1;
        first = model.currentItem - model.itemsOnChart + 1;
        if (first < 0)
            first = 0;
        for (var i = first; i <= last; i++) {
            yo = dataModel.valueToY(dataModel.dataSeries.open[i]);
            yh = dataModel.valueToY(dataModel.dataSeries.high[i]);
            yl = dataModel.valueToY(dataModel.dataSeries.low[i]);
            yc = dataModel.valueToY(dataModel.dataSeries.close[i]);
            x = (k * (model.itemWidth + model.itemsInterval) + dataModel.frame.left);
            lx = (x - (model.itemWidth - 1) / 2);
            rx = (x + (model.itemWidth - 1) / 2);
            k++;
            if (model.itemWidth == 1) {
                graphics.moveTo(x, yh);
                graphics.lineTo(x, yl);
                continue;
            }
            if (yc > yo) {
                graphics.beginFill(0x000000);
                graphics.drawRect(lx, yo, model.itemWidth - 1, Math.abs(yo - yc));
                graphics.endFill();
                graphics.drawRect(lx, yo, model.itemWidth - 1, Math.abs(yo - yc));
                graphics.moveTo(x, yo);
                graphics.lineTo(x, yh);
                graphics.moveTo(x, yc);
                graphics.lineTo(x, yl);
                continue;
            }
            else if (yc < yo) {
                graphics.beginFill(0xffffff);
                graphics.drawRect(lx, yc, model.itemWidth - 1, Math.abs(yc - yo));
                graphics.endFill();
                graphics.drawRect(lx, yc, model.itemWidth - 1, Math.abs(yc - yo));
                graphics.moveTo(x, yo);
                graphics.lineTo(x, yl);
                graphics.moveTo(x, yc);
                graphics.lineTo(x, yh);
                continue;
            }
            else if (yc == yo) {
                graphics.moveTo(x, yh);
                graphics.lineTo(x, yl);
                graphics.moveTo(lx, yo);
                graphics.lineTo(rx, yo);
            }
        }
    };
    return ChartRenderer;
})();
$.get('/data/spy.csv').done(function (data) {
    var opt = {};
    var symbol = new Symbol("spy", TimeFrame.D, 0.01);
    opt.transform = function (obj) {
        symbol.date.unshift(new Date(obj[0]));
        symbol.open.unshift(obj[1]);
        symbol.high.unshift(obj[2]);
        symbol.low.unshift(obj[3]);
        symbol.close.unshift(obj[4]);
        symbol.volume.unshift(obj[5]);
        return false;
    };
    Csv.toObjects(data, opt);
    var chart = {
        title: "spy",
        enableGrid: true,
        dataSeries: [symbol]
    };
    var renderer = new ChartRenderer();
    renderer.render(chart);
});
