select * from highscoretbl;

drop procedure if exists getGlobalRank;
delimiter $$
	create procedure getGlobalRank()
    begin
    
		select GlobalRank 
		from (
			select
			t.memberID,
			t.highscore,
			rank() over (order by t.highscore desc) as GlobalRank
			FROM (
				SELECT
				memberID,
				MAX(score) AS highScore
					FROM highscoretbl
						GROUP BY memberID
				) as t
			order by GlobalRank
		) as s
		order by GlobalRank;
        
	end $$
delimiter ;
call getGlobalRank();