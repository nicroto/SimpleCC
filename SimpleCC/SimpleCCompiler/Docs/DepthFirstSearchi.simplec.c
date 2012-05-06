graph = [
	[1, 2],
	[3, 4],
	[],
	[5],
	[],
	[]
];
array = [0];
result = [0];
while (arrLength(array) > 0) {
	index = dequeue(array);
	stackPush(array, index);
	children = graph[index];
	if (arrLength(children) > 0) {
		array = arrConcat(children, array);
	}
}