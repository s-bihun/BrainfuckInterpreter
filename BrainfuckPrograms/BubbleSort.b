[bsort.b -- bubble sort
(c) 2016 Daniel B. Cristofani
http://brainfuck.org/]

>>,----------[>>,----------]<<[ ; First line was modified comparing to the original program (>>,[>>,]<<[) because it used \0 as an indicator of the end of an input when most of the programs pass '\n' (ASCII 10) at the end.
[<<]>>>>[
<<[>+<<+>-]
>>[>+<<<<[->]>[<]>>-]
<<<[[-]>>[>+<-]>>[<<<+>>>-]]
>>[[<+>-]>>]<
]<<[>>+<<-]<<
]>>>>[++++++++++.>>] ; This line is also changed to output correct symbols.

[This program sorts the bytes of its input by bubble sort.]