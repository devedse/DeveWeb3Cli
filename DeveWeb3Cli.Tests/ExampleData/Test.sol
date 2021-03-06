// SPDX-License-Identifier: MIT

pragma solidity >=0.4.22 <0.9.0;

contract Test {

    struct Color {
        uint8 R;
        uint8 G;
        uint8 B;
    }

    Color public theBackgroundColor;
    uint public theNumber;
    string public theTxt;

    function create(uint number, Color calldata backgroundColor, string calldata txt) public {
        theNumber = number;
        theBackgroundColor = backgroundColor;
        theTxt = txt;
    }
}