var graph = [
	[1, 2],
	[3, 4],
	[],
	[5],
	[],
	[]
];
function depthFirstSearch(n, graph, resultArray) {
	var eds = graph[n];
	if (eds.length > 0) {
		for (var i = 0; i < eds.length; i++) {
			var childIndex = eds[i];
			resultArray.push(childIndex);
			depthFirstSearch(childIndex, graph, resultArray);
		}
	}
}