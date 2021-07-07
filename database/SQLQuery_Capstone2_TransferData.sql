USE tenmo
GO

BEGIN TRANSACTION
INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount)
VALUES (2, 2, 1, 1002, 250.00)
UPDATE accounts SET balance = (balance - 250.00) WHERE user_id = 1
UPDATE accounts SET balance = (balance + 250.00) WHERE user_id = 2
INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount)
VALUES (2, 2, 1002, 1, 100.00)
UPDATE accounts SET balance = (balance - 100.00) WHERE user_id = 2
UPDATE accounts SET balance = (balance + 100.00) WHERE user_id = 1

--SELECT * FROM transfers
--SELECT * FROM accounts

--SELECT DISTINCT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfers AS t
--JOIN accounts AS a ON (a.account_id = t.account_from OR a.account_id = t.account_to)
--JOIN users AS u ON u.user_id = a.user_id;

--SELECT TOP 1 account_id FROM accounts WHERE user_id = 1

--SELECT t.transfer_id, t.transfer_type_id, t.transfer_status_id, t.account_from, t.account_to, t.amount, ts.transfer_status_desc, tt.transfer_type_desc FROM transfers AS t
--JOIN transfer_statuses AS ts ON ts.transfer_status_id = t.transfer_status_id
--JOIN transfer_types AS tt ON tt.transfer_type_id = t.transfer_type_id
--WHERE (1 = t.account_from OR 1 = t.account_to)

--SELECT u.username FROM users As u JOIN accounts AS a ON a.user_id = u.user_id WHERE a.account_id = 1

--BEGIN TRANSACTION
--UPDATE accounts SET balance = (balance + 9999) WHERE account_id = 1;
--UPDATE accounts SET balance = (balance - 9999) WHERE account_id = 1002;
--UPDATE transfers SET transfer_status_id = 2 WHERE transfer_id = 1039;
--COMMIT

SELECT * FROM transfers
SELECT * FROM accounts

ROLLBACK