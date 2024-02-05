>+>>,>,><<   ; Prepare memory and read numbers
+>+<        ; This is for managing if a=0 and b=0
[->-[>]<<]  ; This is a magic loop. if a is the one which reaches 0 first (a<b),then pointer will be at (4). Else it will be at (3)
<[-  
    ; BLOCK (a>=b)
    ; You are at (2) and do whatever you want and come back to (2).
    ; Its a must
]
<[-<
    ; BLOCK(a<b)
    ; You are at (1) and do whatever you want and come back to (1).
    ; Its a must
]
; End of the program.