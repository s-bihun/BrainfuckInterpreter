; Enter boolean value 0 or 1 (actually anything except of 0 is considered as True).
,
; {STOP_DEBUGGING}
------------------------------------------------ ; Entered value is converted to a number from a char and should remain unchanged in it's 'input' cell {START_DEBUGGING} {STOP_DEBUGGING}
[>+>+<<-]>[<+>-]>[<+>[-]]< ; Place 1 to the second ('condition') sell if input means true (not 0) and 0 if input means false (0) {START_DEBUGGING}
>+< ; Prepare the consequent ('auxiliary') cell, 1 should be set there.
; Trying to enter 'then' section.
[
; Start of the 'then' section, pointer is pointing to the condition cell. Return pointer to where it was after an action.
>>+++++++++++<<
; End of the 'then' section
[-] ; Reset value of the condition cell to 0.
>[-] ; Reset value of the auxiliary cell to 0.
< ; Return pointer to the condition cell.
]
> ; Go to the auxiliary cell. The trick is it's value will remain 1 if condition is 0 (false) and 'then' section is skipped. Otherwise it's value remains as 1 and we'll reach 'else' section
[<
; Start of the 'else' section, pointer is pointing to the condition cell. Return pointer to where it was after an action.
>>++++++++++<<
; End of the 'else' section.
>[-] ; Reset value of the auxiliary cell to 0 to exit the 'else' section.
]<< ; Return to the first cell
; End of the program. Should be pointing to the first cell. First ('input') cell is unchanged, second and third ('condition' and 'auxiliary') are 0s. 11 or 10 is added to the 4th cell (depending on condition value prompted).