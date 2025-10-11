
local li = require("luainterop")
require("test2")
-- local t2 = require("test2")

li.log('Loading test1.lua')

-- Test event.
li.notif(111, 'Notification from test1.lua', true, 123.45)


local function boomer1(tt)
    v = boomer2(tt)
    -- v = t2.boomer2(tt)
    return #v
end

function setup(arg)
    boomer1('shakalaka')
end
