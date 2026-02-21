-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2026. Feb 21. 14:05
-- Kiszolgáló verziója: 10.4.20-MariaDB
-- PHP verzió: 7.3.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `acsolas`
--
CREATE DATABASE IF NOT EXISTS `acsolas` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `acsolas`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `appointment`
--

CREATE TABLE `appointment` (
  `Appointment_Id` int(11) NOT NULL,
  `Client_Id` int(11) NOT NULL,
  `Start_Time` datetime NOT NULL,
  `End_Time` datetime NOT NULL,
  `Status` int(11) NOT NULL,
  `Comment` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `category`
--

CREATE TABLE `category` (
  `Category_Id` int(11) NOT NULL,
  `Category_Name` varchar(40) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `category`
--

INSERT INTO `category` (`Category_Id`, `Category_Name`) VALUES
(1, 'Faanyag'),
(2, 'Facsavarok'),
(5, 'Foliak'),
(3, 'Szerkezetepito csavarok'),
(4, 'Tomitok');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `client`
--

CREATE TABLE `client` (
  `Client_Id` int(11) NOT NULL,
  `Name` varchar(30) NOT NULL,
  `Email` varchar(40) NOT NULL,
  `Telephone` varchar(40) NOT NULL,
  `Billing_Address` varchar(50) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `PasswordSalt` varchar(255) NOT NULL,
  `PasswordIterations` int(11) NOT NULL DEFAULT 150000,
  `TokenVersion` int(11) NOT NULL DEFAULT 1,
  `Role` varchar(20) NOT NULL DEFAULT 'User',
  `IsActive` tinyint(1) NOT NULL DEFAULT 1,
  `CreatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `UpdatedAt` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `client`
--

INSERT INTO `client` (`Client_Id`, `Name`, `Email`, `Telephone`, `Billing_Address`, `PasswordHash`, `PasswordSalt`, `PasswordIterations`, `TokenVersion`, `Role`, `IsActive`, `CreatedAt`, `UpdatedAt`) VALUES
(1, 'béla', 'admin@acsolas.com', '21234', 'dtreet12', '', '', 150000, 1, 'User', 1, '2026-02-19 13:31:06', '2026-02-19 13:31:06'),
(2, 'alma', 'alma@example.com', '01202020', 'fujebfubewubf', 'Ha90U1Q3qGW3jwJofrck9iWyjuq81nzjog5LZ6VySp8=', 'UIhhayaYcacQmUBfQ/4b0w==', 150000, 1, 'Admin', 1, '2026-02-19 13:31:06', '2026-02-20 18:51:12'),
(3, 'géza', 'geza@exapmle.com', '06206666666', 'miskolc', 'YgiiDccWv4Zx2lN50tURcl83juUz9H6BGEKPeEHgWcA=', '4Nanloxl14/FAuZEkLs/kA==', 150000, 0, 'User', 1, '0001-01-01 00:00:00', '0001-01-01 00:00:00'),
(4, 'tomi', 'tomi@example.com', '2134124124', 'miskolc', 'SwvfrO/6KFEHcOPU58uhHUNsmVjf20K+WM3GbBKmZ28=', 'NTkUzdG3kbeTSggnimD+DQ==', 150000, 0, 'User', 1, '0001-01-01 00:00:00', '0001-01-01 00:00:00');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `order`
--

CREATE TABLE `order` (
  `Order_Id` int(40) NOT NULL,
  `Client_Id` int(11) NOT NULL,
  `Order_Date` datetime NOT NULL,
  `Order_Status` varchar(20) NOT NULL,
  `Comment` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `order`
--

INSERT INTO `order` (`Order_Id`, `Client_Id`, `Order_Date`, `Order_Status`, `Comment`) VALUES
(1, 1, '2026-02-09 16:19:40', 'Confirmed', 'Első teszt rendelés');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `order_item`
--

CREATE TABLE `order_item` (
  `Order_Item_Id` int(11) NOT NULL,
  `Order_Id` int(11) NOT NULL,
  `Tax_Rate` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL,
  `Unit_Price` decimal(11,0) NOT NULL,
  `Product_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `order_item`
--

INSERT INTO `order_item` (`Order_Item_Id`, `Order_Id`, `Tax_Rate`, `Quantity`, `Unit_Price`, `Product_Id`) VALUES
(1, 1, 27, 2, '10000', 2);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `product`
--

CREATE TABLE `product` (
  `Product_Id` int(11) NOT NULL,
  `Product_Code` varchar(60) NOT NULL,
  `Category_Id` int(11) NOT NULL,
  `Product_Name` varchar(200) NOT NULL,
  `Unit_Type` int(10) NOT NULL,
  `Net_Price` decimal(11,0) NOT NULL,
  `product_group` varchar(50) NOT NULL DEFAULT 'UNSET'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `product`
--

INSERT INTO `product` (`Product_Id`, `Product_Code`, `Category_Id`, `Product_Name`, `Unit_Type`, `Net_Price`, `product_group`) VALUES
(2, 'WOOD-001', 1, 'Lucfenyő gerenda 5m', 1, '10000', 'GERENDA'),
(3, 'SCHMID-RAPID-SSF-6x100-100', 3, 'Schmid RAPID SSF/WH 6,0 x 100/60 - 100db', 0, '5140', 'SCHMID_CSAVAR'),
(5, 'SCHMID-RAPID-SSF-8x200-50', 3, 'Schmid RAPID SSF/WH 8,0 x 200/100 - 50db', 0, '6850', 'SCHMID_CSAVAR'),
(6, 'SCHMID-RAPID-SSF-10x200-25', 3, 'Schmid RAPID SSF/WH 10,0 x 200/100 - 25db', 0, '7874', 'SCHMID_CSAVAR'),
(7, 'RAPID-SULLY-FEJ-SZERK', 2, 'RAPID süllyesztett fejű szerkezeti facsavar', 0, '10550', 'RAPID_FACSAVAR'),
(8, 'AEROSANA-VISCONN-FIBRE-600ML', 4, 'AEROSANA VISCONN fibre FEHÉR 600 ml', 0, '8000', 'AEROSAN_FIBRE'),
(9, 'AEROSANA-VISCONN-FIBRE-5L', 4, 'AEROSANA VISCONN fibre FEHÉR 5 L', 0, '52000', 'AEROSAN_FIBRE'),
(10, 'TESCON-PRIMER-RP-1L', 4, 'Pro Clima TESCON Primer RP 1 L', 0, '10000', 'TESCON_PRIMER'),
(11, 'TESCON-PRIMER-RP-2_5L', 4, 'Pro Clima TESCON Primer RP 2.5 L', 0, '23000', 'TESCON_PRIMER'),
(12, 'AEROSANA-VISCONN-600ML', 5, 'AEROSANA VISCONN légtömör fólia 600 ml', 0, '7000', 'AEROSAN_MEMBRANE'),
(13, 'AEROSANA-VISCONN-10L', 5, 'AEROSANA VISCONN légtömör fólia 10 L', 0, '99000', 'AEROSAN_MEMBRANE'),
(14, 'TESCON-RAPIC-50x30', 4, 'TESCON RAPIC 50 mm x 30 m', 0, '11000', 'TESCON_RAPIC'),
(15, 'TESCON-RAPIC-60x30', 4, 'TESCON RAPIC 60 mm x 30 m', 0, '14000', 'TESCON_RAPIC');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `revokedtokens`
--

CREATE TABLE `revokedtokens` (
  `RevokedToken_Id` int(11) NOT NULL,
  `Jti` varchar(64) NOT NULL,
  `Client_Id` int(11) NOT NULL,
  `RevokedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `ExpiresAt` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `revokedtokens`
--

INSERT INTO `revokedtokens` (`RevokedToken_Id`, `Jti`, `Client_Id`, `RevokedAt`, `ExpiresAt`) VALUES
(1, '47a53051-c833-4aa1-a57a-5d525e116d1e', 2, '2026-02-19 14:44:01', '2026-02-19 15:13:10'),
(2, '126eacdb-cb5a-43d5-b61c-305f6f8c7d35', 2, '2026-02-19 18:51:52', '2026-02-19 19:21:23'),
(3, 'e1a42405-6f6a-4084-9d34-42a929fd0a64', 2, '2026-02-19 19:32:48', '2026-02-19 20:02:44'),
(4, 'db598223-77b3-41e2-8f33-deb27a70bed6', 4, '2026-02-19 19:50:56', '2026-02-19 20:20:48'),
(5, 'c1c4c4ca-6b45-4f23-a4eb-326654f54c21', 4, '2026-02-19 19:51:45', '2026-02-19 20:21:28'),
(6, 'cc7fa8da-7dfc-4002-bffd-76252ec4b611', 2, '2026-02-20 08:20:10', '2026-02-20 08:49:39'),
(7, '7990f199-a5c2-45af-9d19-468f971a2007', 2, '2026-02-20 08:59:12', '2026-02-20 09:22:59'),
(8, '76ffcd20-2fca-445e-b9a7-e770db3dd800', 2, '2026-02-20 18:05:44', '2026-02-20 18:29:03');

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `appointment`
--
ALTER TABLE `appointment`
  ADD PRIMARY KEY (`Appointment_Id`),
  ADD KEY `Client_Id` (`Client_Id`);

--
-- A tábla indexei `category`
--
ALTER TABLE `category`
  ADD PRIMARY KEY (`Category_Id`),
  ADD KEY `Category_Name` (`Category_Name`);

--
-- A tábla indexei `client`
--
ALTER TABLE `client`
  ADD PRIMARY KEY (`Client_Id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- A tábla indexei `order`
--
ALTER TABLE `order`
  ADD PRIMARY KEY (`Order_Id`),
  ADD KEY `Order_Id` (`Order_Id`,`Client_Id`),
  ADD KEY `Client_Id` (`Client_Id`);

--
-- A tábla indexei `order_item`
--
ALTER TABLE `order_item`
  ADD PRIMARY KEY (`Order_Item_Id`),
  ADD KEY `Order_Id` (`Order_Id`),
  ADD KEY `fk_order_item_product` (`Product_Id`);

--
-- A tábla indexei `product`
--
ALTER TABLE `product`
  ADD PRIMARY KEY (`Product_Id`),
  ADD KEY `Product_Code` (`Product_Code`),
  ADD KEY `Category_Id` (`Category_Id`);

--
-- A tábla indexei `revokedtokens`
--
ALTER TABLE `revokedtokens`
  ADD PRIMARY KEY (`RevokedToken_Id`),
  ADD UNIQUE KEY `UQ_RevokedTokens_Jti` (`Jti`),
  ADD KEY `IX_RevokedTokens_ExpiresAt` (`ExpiresAt`),
  ADD KEY `FK_RevokedTokens_Client` (`Client_Id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `appointment`
--
ALTER TABLE `appointment`
  MODIFY `Appointment_Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `category`
--
ALTER TABLE `category`
  MODIFY `Category_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `client`
--
ALTER TABLE `client`
  MODIFY `Client_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT a táblához `order`
--
ALTER TABLE `order`
  MODIFY `Order_Id` int(40) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT a táblához `order_item`
--
ALTER TABLE `order_item`
  MODIFY `Order_Item_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT a táblához `product`
--
ALTER TABLE `product`
  MODIFY `Product_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT a táblához `revokedtokens`
--
ALTER TABLE `revokedtokens`
  MODIFY `RevokedToken_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `appointment`
--
ALTER TABLE `appointment`
  ADD CONSTRAINT `appointment_ibfk_1` FOREIGN KEY (`Client_Id`) REFERENCES `client` (`Client_Id`);

--
-- Megkötések a táblához `order`
--
ALTER TABLE `order`
  ADD CONSTRAINT `order_ibfk_1` FOREIGN KEY (`Client_Id`) REFERENCES `client` (`Client_Id`);

--
-- Megkötések a táblához `order_item`
--
ALTER TABLE `order_item`
  ADD CONSTRAINT `fk_order_item_product` FOREIGN KEY (`Product_Id`) REFERENCES `product` (`Product_Id`),
  ADD CONSTRAINT `order_item_ibfk_1` FOREIGN KEY (`Order_Id`) REFERENCES `order` (`Order_Id`),
  ADD CONSTRAINT `order_item_ibfk_2` FOREIGN KEY (`Product_Id`) REFERENCES `product` (`Product_Id`);

--
-- Megkötések a táblához `product`
--
ALTER TABLE `product`
  ADD CONSTRAINT `product_ibfk_1` FOREIGN KEY (`Category_Id`) REFERENCES `category` (`Category_Id`);

--
-- Megkötések a táblához `revokedtokens`
--
ALTER TABLE `revokedtokens`
  ADD CONSTRAINT `FK_RevokedTokens_Client` FOREIGN KEY (`Client_Id`) REFERENCES `client` (`Client_Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
