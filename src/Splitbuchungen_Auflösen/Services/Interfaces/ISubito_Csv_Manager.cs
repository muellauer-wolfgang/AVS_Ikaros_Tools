﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitbuchungen_Auflösen.Services.Interfaces
{
  public interface ISubito_Csv_Manager
  {
    bool ReadFile(string filename);
    int Find_Column_by_Name(string name);
    IEnumerable<string[]> EnumerateRecords();
    bool WriteFile(string filename);
  }

} //end namespace Splitbuchungen_Auflösen.Services.Interfaces

