-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2026. Feb 18. 17:26
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `product`
--

INSERT INTO `product` (`Product_Id`, `Product_Code`, `Category_Id`, `Product_Name`, `Unit_Type`, `Net_Price`) VALUES
(2, 'WOOD-001', 1, 'Lucfenyő gerenda 5m', 1, 10000),
(3, 'SCHMID-RAPID-SSF-6x100-100', 3, 'Schmid RAPID SSF/WH 6,0 x 100/60 - 100db', 0, 5140),
(4, 'SCHMID-RAPID-SSF-6x120-100', 3, 'Schmid RAPID SSF/WH 6,0 x 120/70 - 100db', 0, 5840),
(5, 'SCHMID-RAPID-SSF-8x200-50', 3, 'Schmid RAPID SSF/WH 8,0 x 200/100 - 50db', 0, 6850),
(6, 'SCHMID-RAPID-SSF-10x200-25', 3, 'Schmid RAPID SSF/WH 10,0 x 200/100 - 25db', 0, 7874),
(7, 'RAPID-SULLY-FEJ-SZERK', 2, 'RAPID süllyesztett fejű szerkezeti facsavar', 0, 10550),
(8, 'AEROSANA-VISCONN-FIBRE-600ML', 4, 'AEROSANA VISCONN fibre FEHÉR 600 ml', 0, 8000),
(9, 'AEROSANA-VISCONN-FIBRE-5L', 4, 'AEROSANA VISCONN fibre FEHÉR 5 L', 0, 52000),
(10, 'TESCON-PRIMER-RP-1L', 4, 'Pro Clima TESCON Primer RP 1 L', 0, 10000),
(11, 'TESCON-PRIMER-RP-2_5L', 4, 'Pro Clima TESCON Primer RP 2.5 L', 0, 23000),
(12, 'AEROSANA-VISCONN-600ML', 5, 'AEROSANA VISCONN légtömör fólia 600 ml', 0, 7000),
(13, 'AEROSANA-VISCONN-10L', 5, 'AEROSANA VISCONN légtömör fólia 10 L', 0, 99000),
(14, 'TESCON-RAPIC-50x30', 4, 'TESCON RAPIC 50 mm x 30 m', 0, 11000),
(15, 'TESCON-RAPIC-60x30', 4, 'TESCON RAPIC 60 mm x 30 m', 0, 14000);

--
-- Indexek a kiírt táblákhoz
--

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
-- AUTO_INCREMENT a táblához `product`
--
ALTER TABLE `product`
  MODIFY `Product_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `product`
--
ALTER TABLE `product`
  ADD CONSTRAINT `product_ibfk_1` FOREIGN KEY (`Category_Id`) REFERENCES `category` (`Category_Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
