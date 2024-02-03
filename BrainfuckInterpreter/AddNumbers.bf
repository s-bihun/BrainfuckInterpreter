>>                          ; reserve 2 cells for the result; current: 2
,>++++++[<-------->-]       ;  read the first number as a character and convert it to a number; current: 3
,>++++++[<-------->-]<      ;  read second number as a character and convert it to a number; current: 3
[<+>-]<                     ;  add second number to the first number in it's cell; current: 2
[<+<+>>-]                   ; copy number from the third cell to the recerved result cells (first two cells); current: 2
++++++++++                  ; prepare 3rd cell (place 10 to it); current: 2
[<<[->>>]>>-]               ; that's how we receive the first result number in a third cell (0 or 1); current: 2
<<[>>++++++++++[<->-]]>>    ; that's how we ensure second result number in a fourth cell is a number 0 to 9; current: 2
++++++[<<++++++++>>-]<<.>>  ; // convert result in the first cells from numbers to the number character; current:
++++++[<++++++++>-]<.       ; // convert result in the second cells from numbers to the number character; current: