WITH CTE AS (
        SELECT id, Name, ManagerID
        FROM Employee
        WHERE id = 1
    UNION ALL
        SELECT t.id, t.Name, t.ManagerID
        FROM Employee t
        INNER JOIN CTE c ON t.ManagerID = c.id
    )
    SELECT * FROM CTE
    ORDER BY ManagerID;