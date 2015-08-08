function NPC_Tutorial_Guide_Deva_contact()
 
	-- 다이얼로그 출력
	dlg_title( "@90100801" )
	dlg_text( "@90100802" )

	dlg_menu( "@90010012", 'GuideTown_JobChange_Deva()' )
	dlg_menu( "@90010013", 'GuideTown_Merchant_Deva()' )
	dlg_menu( "@90010014", 'GuideTown_ItemUp_Deva()' )
	dlg_menu( "@90010015", 'GuideTown_Teleport_Deva()' )
	dlg_menu( "@90010002", '' )
 
	dlg_show()
 
end


function GuideTown_JobChange_Deva()
	dlg_title( "@90100803" )
	dlg_text( "@90100804" )
	
	dlg_menu( "@90010002", '' )
 
	dlg_show()
end

function NPC_Tutorial_Guide_Asura_contact()
 
	-- 다이얼로그 출력
	dlg_title( "@90100801" )
	dlg_text( "@90100802" )

	dlg_menu( "@90010012", 'GuideTown_JobChange_Deva()' )
	dlg_menu( "@90010013", 'GuideTown_Merchant_Deva()' )
	dlg_menu( "@90010014", 'GuideTown_ItemUp_Deva()' )
	dlg_menu( "@90010015", 'GuideTown_Teleport_Deva()' )
	dlg_menu( "@90010002", '' )
 
	dlg_show()
end

function NPC_Tutorial_Guide_Gaia_contact()
 
	-- 다이얼로그 출력
	dlg_title( "@90100801" )
	dlg_text( "@90100802" )

	dlg_menu( "@90010012", 'GuideTown_JobChange_Deva()' )
	dlg_menu( "@90010013", 'GuideTown_Merchant_Deva()' )
	dlg_menu( "@90010014", 'GuideTown_ItemUp_Deva()' )
	dlg_menu( "@90010015", 'GuideTown_Teleport_Deva()' )
	dlg_menu( "@90010002", '' )
 
	dlg_show()
end

function GuideTown_Merchant_Deva()
	dlg_title( "@90100803" )
	dlg_text( "@90100804" )
	
	dlg_menu( "@90010002", '' )
 
	dlg_show()
end

function GuideTown_ItemUp_Deva()
	dlg_title( "@90100803" )
	dlg_text( "@90100804" )
	
	dlg_menu( "@90010002", '' )
 
	dlg_show()
end

function GuideTown_Teleport_Deva()
	dlg_title( "@90100803" )
	dlg_text( "@90100804" )
	
	dlg_menu( "@90010002", '' )
 
	dlg_show()
end