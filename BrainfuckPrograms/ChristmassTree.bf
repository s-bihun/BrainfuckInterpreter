>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> ; somehow it reads negative memory cells (it's a temporary fix). Also it seem to break my debugger, let's take a look some time.

; taken from https://seblink.wordpress.com/2013/12/17/christmas-the-brainfuck-way/
; though, it works for example with this compiler: https://sange.fi/esoteric/brainfuck/impl/interp/i.html though it seems to be not affected by the input parameter

>>>>>,>++<<<<<<>[-]>[-]>[-]>[-]>[<<<<+>>>>-]<<<<[>>>>>[
<<<<+>+>>>-]<<<[>>>+<<<-]<[>+<<-[>>[-]>+<<<-]>>>[<<<+>>
>-]<[<-[>>>-<<<[-]]+>-]<-]>>>+<<<<]>>>>>>+<<>>>+++++++[
>+++++<-]>---<<<<>>>+++++++[>>+++++<<-]<<<>>>++++++++++
<<<[->>>>>>+>+>>>>>>>>>+<<<<<<<<<<<<<<<<]>>[->>>>>>>+<<
<<<<<]>>>>+[>[->+>>>+>>+<<<<<<]>>[->+>>+<<<]>>[-<<<<<<<
.>>>>>>>]>[-<<<<<<<.>>>>>>>]>[-<<<<<<<<<.>>>>>>>>>]<<<<
<<<<<<.>>>>>>>>>><<<<<[-<+>]<->>>[-<+>]<++<<<-]>>>>>>>>
>>-[<<<<<<<<<<<<.>>>>>>>>>>>>-]<<<<<<<<<<<...