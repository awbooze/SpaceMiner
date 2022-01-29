//  Copyright (C) 2022 Andrew Booze <42682253+awbooze@users.noreply.github.com>
//
//  This Source Code Form is subject to the terms of the Mozilla Public
//  License, v. 2.0. If a copy of the MPL was not distributed with this
//  file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;

namespace SpaceMiner
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SpaceMinerGame())
                game.Run();
        }
    }
}
