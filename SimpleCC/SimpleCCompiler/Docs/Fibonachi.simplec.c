n = scanf() - 1;
fib = [1, 1];
counter = 2;
while(counter < n) {
	arrPush(fib, (fib[counter - 1] + fib[counter - 2]));
	counter++;
}
printf(fib[n]);