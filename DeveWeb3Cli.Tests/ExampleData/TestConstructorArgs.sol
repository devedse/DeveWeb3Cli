// SPDX-License-Identifier: MIT

pragma solidity >=0.4.22 <0.9.0;

contract TestConstructorArgs {

    struct Color {
        uint8 R;
        uint8 G;
        uint8 B;
    }

    Color public theBackgroundColor;
    uint public theNumber;
    string public theTxt;

    constructor(uint number, Color memory backgroundColor, string memory txt) payable {
        require(msg.value >= 0.001 ether, "Payment is not enough");

        theNumber = number;
        theBackgroundColor = backgroundColor;
        theTxt = txt;
    }
}