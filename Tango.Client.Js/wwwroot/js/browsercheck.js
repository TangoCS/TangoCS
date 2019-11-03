var str = 'const a = 1';

try {
	eval(str);
} catch (e) {
	window.location = '/notsupported.html'
}