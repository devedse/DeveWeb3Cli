// SPDX-License-Identifier: MIT

pragma solidity >=0.4.22 <0.9.0;

contract Test {

    struct Color {
        uint8 R;
        uint8 G;
        uint8 B;
    }

    Color public theBackgroundColor;
    uint theNumber;

    function create(uint number, Color calldata backgroundColor) public {
        theNumber = number;
        theBackgroundColor = backgroundColor;
    }
}