-- MySQL dump 10.13  Distrib 5.7.5-m15, for Win64 (x86_64)
--
-- Host: localhost    Database: accountdb
-- ------------------------------------------------------
-- Server version	5.7.5-m15-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bank_deposits`
--

DROP TABLE IF EXISTS `bank_deposits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bank_deposits` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) NOT NULL DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `amount` int(11) NOT NULL,
  `financial_code` varchar(4) NOT NULL,
  `status` varchar(10) NOT NULL,
  `bank_code` varchar(20) NOT NULL,
  `bank` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bank_deposits`
--

LOCK TABLES `bank_deposits` WRITE;
/*!40000 ALTER TABLE `bank_deposits` DISABLE KEYS */;
/*!40000 ALTER TABLE `bank_deposits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bank_withdrawals`
--

DROP TABLE IF EXISTS `bank_withdrawals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bank_withdrawals` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) NOT NULL DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `amount` int(11) NOT NULL,
  `financial_code` varchar(4) NOT NULL,
  `status` varchar(10) NOT NULL,
  `bank_code` varchar(20) NOT NULL,
  `bank` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bank_withdrawals`
--

LOCK TABLES `bank_withdrawals` WRITE;
/*!40000 ALTER TABLE `bank_withdrawals` DISABLE KEYS */;
/*!40000 ALTER TABLE `bank_withdrawals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cash_payments`
--

DROP TABLE IF EXISTS `cash_payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cash_payments` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) NOT NULL,
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `amount` int(11) NOT NULL DEFAULT '0',
  `financial_code` varchar(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cash_payments`
--

LOCK TABLES `cash_payments` WRITE;
/*!40000 ALTER TABLE `cash_payments` DISABLE KEYS */;
/*!40000 ALTER TABLE `cash_payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cash_receipts`
--

DROP TABLE IF EXISTS `cash_receipts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cash_receipts` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) NOT NULL DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `amount` int(11) NOT NULL DEFAULT '0',
  `financial_code` varchar(4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cash_receipts`
--

LOCK TABLES `cash_receipts` WRITE;
/*!40000 ALTER TABLE `cash_receipts` DISABLE KEYS */;
/*!40000 ALTER TABLE `cash_receipts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `journal_vouchers`
--

DROP TABLE IF EXISTS `journal_vouchers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `journal_vouchers` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) NOT NULL DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `debit` int(11) NOT NULL DEFAULT '0',
  `credit` int(11) NOT NULL DEFAULT '0',
  `financial_code` varchar(4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `journal_vouchers`
--

LOCK TABLES `journal_vouchers` WRITE;
/*!40000 ALTER TABLE `journal_vouchers` DISABLE KEYS */;
/*!40000 ALTER TABLE `journal_vouchers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ledger_register`
--

DROP TABLE IF EXISTS `ledger_register`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ledger_register` (
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `alternate_name` varchar(200) DEFAULT NULL,
  `address1` varchar(100) DEFAULT NULL,
  `address2` varchar(100) DEFAULT NULL,
  `address3` varchar(100) DEFAULT NULL,
  `phone` varchar(30) DEFAULT NULL,
  `pin_no` varchar(10) DEFAULT NULL,
  `state` varchar(100) DEFAULT NULL,
  `account_no` varchar(45) DEFAULT NULL,
  `pan_no` varchar(20) DEFAULT NULL,
  `sale_tax_no` varchar(20) DEFAULT NULL,
  `tin_no` varchar(20) DEFAULT NULL,
  `is_editable` bit(1) DEFAULT b'0',
  `is_removable` bit(1) DEFAULT b'0',
  `is_enabled` bit(1) DEFAULT b'0',
  `type` varchar(10) NOT NULL,
  `group_code` varchar(20) DEFAULT NULL,
  `special_code` varchar(20) DEFAULT NULL,
  `unique_id` varchar(100) DEFAULT NULL,
  `narration` varchar(200) DEFAULT NULL,
  `a_group_code` varchar(20) DEFAULT NULL,
  `b_group_code` varchar(20) DEFAULT NULL,
  `c_group_code` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`ledger_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ledger_register`
--

LOCK TABLES `ledger_register` WRITE;
/*!40000 ALTER TABLE `ledger_register` DISABLE KEYS */;
/*!40000 ALTER TABLE `ledger_register` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ledger_transactions`
--

DROP TABLE IF EXISTS `ledger_transactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ledger_transactions` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `bill_type` varchar(3) NOT NULL,
  `serial_no` int(11) NOT NULL DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `debit` int(11) DEFAULT '0',
  `credit` int(11) DEFAULT '0',
  `a_group_code` varchar(20) DEFAULT NULL,
  `b_group_code` varchar(20) DEFAULT NULL,
  `c_group_code` varchar(20) DEFAULT NULL,
  `co_ledger` varchar(100) DEFAULT NULL,
  `ref_bill_no` varchar(20) DEFAULT NULL,
  `ref_bill_date_time` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ledger_transactions`
--

LOCK TABLES `ledger_transactions` WRITE;
/*!40000 ALTER TABLE `ledger_transactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `ledger_transactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `opening_balances`
--

DROP TABLE IF EXISTS `opening_balances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `opening_balances` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) NOT NULL DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `debit` int(11) NOT NULL DEFAULT '0',
  `credit` int(11) NOT NULL DEFAULT '0',
  `financial_code` varchar(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `opening_balances`
--

LOCK TABLES `opening_balances` WRITE;
/*!40000 ALTER TABLE `opening_balances` DISABLE KEYS */;
/*!40000 ALTER TABLE `opening_balances` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase`
--

DROP TABLE IF EXISTS `purchase`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `purchase` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) NOT NULL DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `debit` int(11) NOT NULL DEFAULT '0',
  `credit` int(11) NOT NULL DEFAULT '0',
  `financial_code` varchar(4) NOT NULL,
  `ref_bill_no` varchar(20) DEFAULT NULL,
  `ref_bill_date_time` datetime DEFAULT NULL,
  `party_code` varchar(20) DEFAULT NULL,
  `party` varchar(100) DEFAULT NULL,
  `purchase_ledger_code` varchar(20) DEFAULT NULL,
  `purchase_ledger` varchar(100) DEFAULT NULL,
  `bill_amount` int(11) DEFAULT NULL,
  `debit_balance` int(11) DEFAULT NULL,
  `credit_balance` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase`
--

LOCK TABLES `purchase` WRITE;
/*!40000 ALTER TABLE `purchase` DISABLE KEYS */;
/*!40000 ALTER TABLE `purchase` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `purchase_return`
--

DROP TABLE IF EXISTS `purchase_return`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `purchase_return` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) NOT NULL DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `debit` int(11) DEFAULT NULL,
  `credit` int(11) DEFAULT NULL,
  `financial_code` varchar(4) NOT NULL,
  `ref_bill_no` varchar(20) DEFAULT NULL,
  `ref_bill_date_time` datetime DEFAULT NULL,
  `party_code` varchar(20) DEFAULT NULL,
  `party` varchar(100) DEFAULT NULL,
  `purchase_ledger_code` varchar(20) DEFAULT NULL,
  `purchase_ledger` varchar(100) DEFAULT NULL,
  `bill_amount` int(11) DEFAULT NULL,
  `debit_balance` int(11) DEFAULT NULL,
  `credit_balance` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase_return`
--

LOCK TABLES `purchase_return` WRITE;
/*!40000 ALTER TABLE `purchase_return` DISABLE KEYS */;
/*!40000 ALTER TABLE `purchase_return` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales`
--

DROP TABLE IF EXISTS `sales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sales` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `debit` int(11) DEFAULT NULL,
  `credit` int(11) DEFAULT NULL,
  `financial_code` varchar(4) NOT NULL,
  `ref_bill_no` varchar(20) DEFAULT NULL,
  `ref_bill_date_time` datetime DEFAULT NULL,
  `party_code` varchar(20) DEFAULT NULL,
  `party` varchar(100) DEFAULT NULL,
  `sales_ledger_code` varchar(20) DEFAULT NULL,
  `sales_ledger` varchar(100) DEFAULT NULL,
  `bill_amount` int(11) DEFAULT NULL,
  `debit_balance` int(11) DEFAULT NULL,
  `credit_balance` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales`
--

LOCK TABLES `sales` WRITE;
/*!40000 ALTER TABLE `sales` DISABLE KEYS */;
/*!40000 ALTER TABLE `sales` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales_return`
--

DROP TABLE IF EXISTS `sales_return`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sales_return` (
  `bill_no` varchar(20) NOT NULL DEFAULT '1',
  `bill_date_time` datetime NOT NULL,
  `serial_no` int(11) DEFAULT '1',
  `ledger_code` varchar(20) NOT NULL,
  `ledger` varchar(100) NOT NULL,
  `narration` varchar(150) DEFAULT NULL,
  `debit` int(11) DEFAULT NULL,
  `credit` int(11) DEFAULT NULL,
  `financial_code` varchar(4) NOT NULL,
  `ref_bill_no` varchar(20) DEFAULT NULL,
  `ref_bill_date_time` datetime DEFAULT NULL,
  `party_code` varchar(20) DEFAULT NULL,
  `party` varchar(100) DEFAULT NULL,
  `sales_ledger_code` varchar(20) DEFAULT NULL,
  `sales_ledger` varchar(100) DEFAULT NULL,
  `bill_amount` int(11) DEFAULT NULL,
  `debit_balance` int(11) DEFAULT NULL,
  `credit_balance` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales_return`
--

LOCK TABLES `sales_return` WRITE;
/*!40000 ALTER TABLE `sales_return` DISABLE KEYS */;
/*!40000 ALTER TABLE `sales_return` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-12-29 12:53:05
