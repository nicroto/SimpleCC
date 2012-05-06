var dp = [1, 1];
function fib(n) {
	var i = n - 1;
	if (dp[i]) return dp[i];
	return (dp[i] = fib(i - 1) + fib(i - 2));
}
console.log(fib(5));