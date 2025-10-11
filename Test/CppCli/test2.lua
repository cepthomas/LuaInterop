
local li  = require("luainterop")

li.log('Loading test2.lua')

-- Test event.
li.notif(222, "Notification from test2", false, 707.07)

-- uncomment to cause syntax error:
-- bad boy

function boomer2(tt)
    -- Cause a fault.
    xxx = 'boom' + nil
end
