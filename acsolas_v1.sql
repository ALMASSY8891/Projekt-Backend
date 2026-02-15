-- phpMyAdmin SQL Dump
-- version 5.1.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2026. Feb 15. 10:41
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
  `Password` varchar(255) NOT NULL,
  `Telephone` varchar(40) NOT NULL,
  `Billing_Address` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `client`
--

INSERT INTO `client` (`Client_Id`, `Name`, `Email`, `Password`, `Telephone`, `Billing_Address`) VALUES
(1, 'béla', 'admin@acsolas.com', 'admin', '21234', 'dtreet12');

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
  `Net_Price` decimal(11,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- A tábla adatainak kiíratása `product`
--

INSERT INTO `product` (`Product_Id`, `Product_Code`, `Category_Id`, `Product_Name`, `Unit_Type`, `Net_Price`) VALUES
(2, 'WOOD-001', 1, 'Lucfenyő gerenda 5m', 1, '10000'),
(3, 'SCHMID-RAPID-SSF-6x100-100', 3, 'Schmid RAPID SSF/WH 6,0 x 100/60 - 100db', 0, '5140'),
(4, 'SCHMID-RAPID-SSF-6x120-100', 3, 'Schmid RAPID SSF/WH 6,0 x 120/70 - 100db', 0, '5140'),
(5, 'SCHMID-RAPID-SSF-8x200-50', 3, 'Schmid RAPID SSF/WH 8,0 x 200/100 - 50db', 0, '5140'),
(6, 'SCHMID-RAPID-SSF-10x200-25', 3, 'Schmid RAPID SSF/WH 10,0 x 200/100 - 25db', 0, '5140'),
(7, 'RAPID-SULLY-FEJ-SZERK', 2, 'RAPID sullyesztett feju szerkezeti facsavar', 0, '10954'),
(8, 'AEROSANA-VISCONN-FIBRE-600ML', 4, 'AEROSANA VISCONN fibre FEHER 600 ml', 0, '8000'),
(9, 'AEROSANA-VISCONN-FIBRE-5L', 4, 'AEROSANA VISCONN fibre FEHER 5 l', 0, '8000'),
(10, 'TESCON-PRIMER-RP-1L', 4, 'Pro Clima TESCON Primer RP 1 L', 0, '10000'),
(11, 'TESCON-PRIMER-RP-2_5L', 4, 'Pro Clima TESCON Primer RP 2.5 L', 0, '10000'),
(12, 'AEROSANA-VISCONN-600ML', 5, 'AEROSANA VISCONN legtömör folia 600 ml', 0, '7000'),
(13, 'AEROSANA-VISCONN-10L', 5, 'AEROSANA VISCONN legtömör folia 10 l', 0, '7000'),
(14, 'TESCON-RAPIC-50x30', 4, 'TESCON RAPIC 50 mm x 30 m', 0, '11000'),
(15, 'TESCON-RAPIC-60x30', 4, 'TESCON RAPIC 60 mm x 30 m', 0, '11000');

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
  MODIFY `Client_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

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
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
