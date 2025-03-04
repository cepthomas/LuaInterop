
local li = require("luainterop")

print("Loading script_example.lua!")

thing1 = 123  --"aaaaaa"


----------------- Functions called from C#. -----------------
function my_lua_func(arg_one, arg_two, arg_three)
    print(">>> my_lua_func")
    local ret = { "A", "B", "C" }
    -- local ret = { 123, "A", "xxx"=6.789 }
    return ret
end 

function my_lua_func2(arg_one)
    print(">>> my_lua_func2")

    local tm = li.get_time()
    print("time is: ", tm)

    local res = li.check_value(123.5, 123.4)
    print("answer is: ", res)

    local res = li.check_value(123.5, 123.6)
    print("answer is: ", res)

    if arg_one then return 99.99 else return 100.01 end
end

function no_args_func()
    print(">>> no_args_func")
    return 88888
end

function optional_func()
    print(">>> optional_func")
    return 12345
end
