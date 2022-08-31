select * from highscoretbl;

drop procedure if exists getHighscoreOrderByRank;
delimiter $$
	create procedure getHighscoreOrderByRank()
    begin
    
		select s.highscore 
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
call getHighscoreOrderByRank();