; Prepare the first cell as it works as a counter {STOP_DEBUGGING}
++++++++++[
; Starting the loop to prepare letters in subsequent cells {START_DEBUGGING(CELL[0]==10)} {STOP_DEBUGGING}
>+++++++>++++++++++>+++<<<-]
; Exiting the loop, new we have prepared cells to output "Hello World!" {START_DEBUGGING}
>++
; Print 'H'
.
>+
; Print 'e'
.
+++++++
; Print double 'l's
..
+++
; Print 'o'
.
>++
; Print ' '
.
<<+++++++++++++++
; Print 'W'
.
>
; Print 'o'
.
+++
; Print 'r'
.
------
; Print 'l'
.
--------
; Print 'd'
.
>+
; Print '!'
.
; Final output should be "Hello World!"