; Find a+b (a and b are digits)
>+>                         ; prepare [0..2] for the 'ge' comparison
>>++++++++++                ; keep [3] empty (reserved for the first number to compare with 10) and place 10 to the [4]
>                           ; reserve [5] for the result
; Input the first digit (a)
>>,>++++++[<-------->-]      ; read the first digit as a character and convert it to a number
; Input the seconde digit (b)
,>++++++[<-------->-]<      ; read second digit as a character and convert it to a number {BREAK}
[<+>-]<                     ; add second number to the first number and keep the result at [7] {BREAK}
[<+<<<+>>>>-]               ; copy the result from [7] to the [6] and [3], clear [6] {BREAK}
<<<<                        ; go to [3] and start comparison of [3] and [4] (10 should be there) to find if the first digit of a result is 0 or 1 {BREAK}
[->-[>]<<]                  ; If result<10 then pointer will stop at [4]. Else it will stop at [3]. {BREAK}
<[
    ; BLOCK (result>=10). You must get back to [2] and reset it's value to 0 at the end of a block.
    >++++++[<++++++++>-]<. ; output 1 (first digit of a result) {BREAK}
    >++++++[<-------->-]<-    
    >>>>>>++++++[<++++++>-]<++. ; output second digit of a result. {BREAK}
    <<<<<
]
<[
    ; BLOCK(result<10). You must get back to [0] and reset it's value to 0 at the end of a block. {BREAK}
    -<
    >>>>>>>++++++[<++++++++>-]<. ; output the result as it's one-digit number {BREAK}
    <<<<<<
]
; End of the program