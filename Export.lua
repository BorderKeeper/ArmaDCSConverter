local default_output_file = nil

function LuaExportStart()
    default_output_file = io.open(lfs.writedir().."/Logs/Export.log", "w")

	default_output_file:write("Time|ID|Name|Country|Side|Lat|Long|Altitude|Heading\n")
end

function LuaExportBeforeNextFrame()

end

function LuaExportAfterNextFrame()

end

function LuaExportStop()
   if default_output_file then
	  default_output_file:close()
	  default_output_file = nil
   end
end

function LuaExportActivityNextEvent(t)
	local tNext = t
	
    if default_output_file then
	    local o = LoGetWorldObjects()
		for k,v in pairs(o) do
			default_output_file:write(string.format("%d|%d|%s|%s|%s|%f|%f|%f|%f\n", t, k, v.Name, v.Country, v.Coalition, v.LatLongAlt.Lat, v.LatLongAlt.Long, v.LatLongAlt.Alt, v.Heading))
		end
    end
	
	tNext = tNext + 1.0
	return tNext
end
pcall(function() local dcsSr=require('lfs');dofile(dcsSr.writedir()..[[Mods\Services\DCS-SRS\Scripts\DCS-SimpleRadioStandalone.lua]]); end,nil);

