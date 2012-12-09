SimpleCC is a project for programming language compiler similar to C (that's where the "SimpleC" comes from).
The targeted language is MSIL (.NET).

This project is simply a "Hello world!" in the compiler/programming language design for me.

As a start I want to create a C-like expression compiler and then to improve it on iterations with new constructs.

Here is SimpleCC-1 spec:
<pre>
Alphabet
------

 Letter = 'a'..'z' | 'A'..'Z' | '_'.
 Digit = '0'..'9'.
 Space = \t | \n | \r | ' '.
 AnyChar = ' '..\0xff.

Lexems
-------

  Number = Digit {Digit}.
  Ident = Letter {Letter | Digit}.
  Delimiter = Space
  Keyword = 'scanf' | 'printf'.
  SpecialSymbol = '=' | 
                  '(' | ')' | ';' | '+' | '-' | '*' | 
                  '|' | '/' | '%' | '&' | '~' | '++' | '--'.
  OtherSymbol = ...

Grammar
---------

[1] Program = {Statement}.
[2] Statement = [Expression] ';'.
[3] Expression = BitwiseAndExpression {'|' BitwiseAndExpression}.
[4] BitwiseAndExpression = AdditiveExpression {'&' AdditiveExpression}.
[5] AdditiveExpression = MultiplicativeExpression {('+' | '-') MultiplicativeExpression}.
[6] MultiplicativeExpression = PrimaryExpression {('*' | '/' | '%') PrimaryExpression}.
[7] PrimaryExpression = Ident ['=' Expression] | '~' PrimaryExpression | '++' Ident | '--' Ident | Ident '++' | Ident '--' | Number | PrintFunc | ScanfFunc | '(' Expression ')'.
[8] PrintFunc = 'printf' '(' Expression ')'.
[9] ScanfFunc = 'scanf' '(' ')'.

Tables
-------

  Reserved identifiers (Name, Value, ...)

  Embedded types: { int }

Examples
-------

A = scanf();
B = A*2;
printf(B);


A = scanf();
B = scanf();
printf(A+B);

A = scanf();
B = scanf();
C = (A*A+B*B)*2;
printf(C);

A = scanf();
B = scanf();
A++;
C = A+B;
printf(C);
</pre>

# Update: After Spec-1 was fully implemented here is the new spec that SimpleCC will target:
<pre>
Alphabet
------

 Letter = 'a'..'z' | 'A'..'Z' | '_'.
 Digit = '0'..'9'.
 Space = \t | \n | \r | ' '.
 AnyChar = ' '..\0xff.


Lexemes
-------

  Number = Digit {Digit}.
  Ident = Letter {Letter | Digit}.
  Delimiter = Space | '/*' {AnyChar}\('*' '/') '*/'
  Keyword =	'if' |
		'while' |
		'break'.

  SpecialSymbol =	'=' | '+=' | '-=' | '*=' | '/=' | '%=' | '(' |
			')' | ';' | '+' | '-' | '*' | '|' | '/' | '%' |
			'&' | '~' | '++' | '--' | '<=' | '!=' | '>=' | 
			'==' | '<' | '>' | '||' | '&&' | '!'.
  OtherSymbol = ...


Grammar
---------

[1]  Program = {Statement}.
[2]  Statement =	Block |
			IfSt |
			WhileSt |
			StopSt |
			[Expression] ';'.
[3]  Block = '{' {Statement} '}'
[4]  IfSt = 'if' '(' Expression ')' Statement ['else' Statement].
[5]  WhileSt = 'while' '(' Expression ')' Statement. 
[6]  StopSt =	'break' ';' |
		'continue' ';'.
[7]  Expression =  AdditiveExpr [('<' | '<=' | '==' | '!=' | '>=' | '>') AdditiveExpr].
[8]  AdditiveExpr = ['+' | '-'] MultiplicativeExpr {('+' | '-' | '|' | '||') MultiplicativeExpr}.
[9]  MultiplicativeExpr = SimpleExpr {('*' | '/' | '%' | '&' | '&&') SimpleExpr}.
[10] SimpleExpr = ('++' | '--' | '-' | '~' | '!') PrimaryExpr | PrimaryExpr ['++' | '--'].
[11] PrimaryExpr =	VarIdent '=' ArrayDef |
			['+'|'-'] VarIdent |
			'*' VarIdent |
			'&' VarIdent |
			VarIdent '[' (Number | VarIdent) ']' |
			VarIdent [('='|'+='|'-='|'*='|'/='|'%=') Expression] |
			FuncIdent '(' [Expression] ')' |
			'(' Expression ')'.
[12] ArrayDef = '[' [ArrayItem {',' ArrayItem}] ']'
[13] ArrayItem = (Expression | ArrayDef).


Tables
-------

  Predefined Identifiers (reserved) (Name, Kind, [Type, Offset, Param], ...)

  Embedded:
    types	{ int, bool }
    functions	{
			scanf,
			printf,
			arrPush,
			arrLength,
			arrConcat,
			arrInsert,
			dequeue,
			stackPush
		}


Examples
-------

 - DepthFirstSearch:

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

 - nth Fibonnachi number:

	n = scanf() - 1;
	fib = [1, 1];
	counter = 2;
	while(counter < n) {
		arrPush(fib, (fib[counter - 1] + fib[counter - 2]));
		counter++;
	}
	printf(fib[n]);
</pre>
As you can see there are elements that are getting the grammar away from the C programming language. One major such example is the dynamic array (both as declaration and altering).