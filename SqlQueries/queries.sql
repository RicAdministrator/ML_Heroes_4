USE [HeroDb]

SELECT * FROM dbo.Heroes

SELECT * FROM dbo.Roles

SELECT 
	h.Name,
	r.HeroRole
FROM 
	dbo.HeroRole hr INNER JOIN
	dbo.Heroes h ON hr.HeroesId = h.Id INNER JOIN
	dbo.Roles r ON hr.RolesId = r.Id
ORDER BY 
	HeroesId, RolesId