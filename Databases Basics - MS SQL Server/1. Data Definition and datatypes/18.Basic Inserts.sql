INSERT INTO Towns VALUES
('Sofia'),
('Plovdiv'),
('Varna'),
('Burgas')

INSERT INTO Addresses VALUES
('ул. Тинтява 17', 1),
('Владиславово!', 3),
('Бургас...', 4),
('ул. Победа 51', 2)

INSERT INTO Departments VALUES
('Engineering'), 
('Sales'), 
('Marketing'), 
('Software Development'), 
('Quality Assurance')

INSERT INTO Employees VALUES
('Ivan', 'Ivanov', 'Ivanov','.NET Developer',4,CONVERT([datetime], '01-02-2013',103),3500,1),
('Petar', 'Petrov', 'Petrov','Senior Engineer',1,CONVERT([datetime], '02-03-2014',103),4000,1),
('Maria', 'Petrova', 'Ivanova','Intern',5,CONVERT([datetime], '28-08-2016',103),525.25,1),
('Georgi', 'Teziev', 'Ivanov','CEO',2,CONVERT([datetime], '09-12-2007',103),3000,1),
('Peter', 'Pan', 'Pan','Intern',3,CONVERT([datetime], '28-08-2016',103),599.88,1)