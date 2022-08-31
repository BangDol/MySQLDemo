select * from gachatbl;

drop procedure testPro;
delimiter $$
	create procedure testPro()
    begin
		
        declare percent FLOAT DEFAULT 100.0;
        declare dr FLOAT DEFAULT 1.0;
        declare cumulative FLOAT DEFAULT 0.0;
        declare i INT DEFAULT 0;
        declare _count FLOAT DEFAULT 0;
        
		declare c_gunName char(20);
        declare c_percentage float;
        declare userCursor cursor for
			select gunName, percentage
			from gachatbl;
        
        set dr = rand() * percent;
        set percent = (select sum(percentage) from gachatbl);
        set _count = (select count(*) from gachatbl);
        
        open userCursor;
        myWhile: WHILE (i < _count) DO
				fetch next from userCursor into c_gunName, c_percentage;
                set cumulative = cumulative + c_percentage;
					if (dr <= cumulative) then
					select c_gunName;
                    leave myWhile;
					end if;
                set i = i + 1;
			end while;
		close userCursor;
	end $$
delimiter ;
call testPro();