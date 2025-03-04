
local lb  = require("luainterop")

-----------------------------------------------------------------------------
--- Log functions. This goes straight through to the host.
-- Magic numbers must match host code.
-- @param msg what to log
local function log_error(msg) lb.log(4, msg) end
local function log_warn(msg)  lb.log(3, msg) end
local function log_info(msg)  lb.log(2, msg) end
local function log_debug(msg) lb.log(1, msg) end
local function log_trace(msg) lb.log(0, msg) end

log_info('Loading test.lua')

-- Test event.
lb.notif(33, "XYZ")


-----------------------------------------------------------------------------
function setup()
    log_info('setup()')
    return 5
end


-----------------------------------------------------------------------------
--- Global function for App interaction with script internals.
-- @param cmd specific command string
-- @param arg optional argument string
-- @return result string (table would be nice later)
function do_command(cmd, arg)
    print('>>> do_command', cmd, arg)
    if cmd == 'unload_all' then  -- Unload everything so that the script can be reloaded.
        -- package.loaded.bar_time = nil
        -- package.loaded.debugger = nil
        -- package.loaded.lbot_utils = nil
        -- package.loaded.midi_defs = nil
        -- package.loaded.music_defs = nil
        -- package.loaded.script_api = nil
        -- package.loaded.step_types = nil
        -- package.loaded.stringex = nil
        return '0'
    -- elseif cmd == 'section_info' then -- Return the collected section information.
    --     local res = {}
    --     for k, v in pairs(_section_info) do
    --         table.insert(res, k..','..v)
    --     end
    --     return sx.strjoin('|', res)
    else
        log_info('Unknown cmd '..cmd..' '..arg)
        return 'who knows'
    end
end
