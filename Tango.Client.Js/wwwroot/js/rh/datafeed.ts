abstract class DataFeed {
	abstract loadHistory(symbolName: string): Symbol
}

enum TimeFrame { MN, W, D }

class Symbol {
	name: string;

	onePoint: number;
	period: TimeFrame;

	date: Date[] = [];
	open: number[] = [];
	high: number[] = [];
	low: number[] = [];
	close: number[] = [];
	volume: number[] = [];

	constructor(name: string, period: TimeFrame, onePoint: number) {
		this.name = name;
		this.period = period;
		this.onePoint = onePoint;
	}

	count(): number {
		return this.close.length;
	}
}



