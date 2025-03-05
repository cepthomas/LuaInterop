
local li = require("luainterop")

print("Loading script_example.lua!")

thing1 = 123  --"aaaaaa"

li.log(2, "Hello from luainterop")


----------------- Functions called from C#. -----------------
function my_lua_func(arg_one, arg_two, arg_three)
    li.log(2, "my_lua_func() called")
    local ret = { "A", "B", "C" }
    -- local ret = { 123, "A", "xxx"=6.789 }
    return ret
end 

function my_lua_func2(arg_one)
    local tm = li.get_time()
    print("my_lua_func2() time is: ", tm)

    local res = li.check_value(123.5, 123.4)
    print("my_lua_func2() answer is: ", res)

    local res = li.check_value(123.5, 123.6)
    print("my_lua_func2() answer is: ", res)

    if arg_one then return 99.99 else return 100.01 end
end

function no_args_func()
    print("no_args_func() called")
    return 88888
end

function optional_func()
    print(">>> optional_func")
    return 12345
end
